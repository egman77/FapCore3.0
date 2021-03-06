﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.Extensions;
using SysIO = System.IO;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Annex;
using Fap.Core.Infrastructure.Metadata;
using Fap.AspNetCore.Controls;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using Fap.Core.Infrastructure.Model;
using Fap.Core.DataAccess;
using System.Net.Mime;
using System.IO;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Annex.Utility.Zip;
using System.Text;
using System.Threading.Tasks;
using Fap.ExcelReport;
using NPOI.SS.Converter;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Fap.Core.Office.Excel;

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    [Area("System")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api/Tools")]
    public class ToolsApiController : FapController
    {
        private readonly IExcelReportService _reportService;
        private IFapFileService _fileService;
        private IDbMetadataContext _dbMetadataContext;
        public ToolsApiController(IServiceProvider serviceProvider, IFapFileService fileService, IDbMetadataContext dbMetadataContext, IExcelReportService reportService) : base(serviceProvider)
        {
            _fileService = fileService;
            _dbMetadataContext = dbMetadataContext;
            _reportService = reportService;
        }


        /// <summary>
        /// 获取字典分类
        /// </summary>
        /// <returns></returns>
        [Route("DictCategory")]
        // POST: api/Common
        public JsonResult DictCategory()
        {
            var listCat = _dbContext.Query<FapDict>("select Category,CategoryName from FapDict group by Category,CategoryName");
            List<TreeDataView> oriList = listCat.Select(t => new TreeDataView { Id = t.Category, Pid = "0", Text = t.CategoryName, Icon = "icon-folder orange ace-icon fa fa-leaf" }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "字典分类",
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);

            return Json(tree);
        }

        [Route("DeleteDict/{cat=''}")]
        public JsonResult GetDeleteDict(string cat)
        {
            ResponseViewModel rvm = new ResponseViewModel();
            if (cat.IsMissing())
            {
                rvm.success = false;
                rvm.msg = "字典分类为空，不能删除！";
                return Json(rvm);
            }
            DynamicParameters param = new DynamicParameters();
            param.Add("Category", cat);
            int count = _dbContext.Count("FapColumn", " CtrlType='COMBOBOX' and RefTable=@Category", param);
            if (count > 0)
            {
                rvm.success = false;
                rvm.msg = "此编码已经被使用，不能删除！";
                return Json(rvm);
            }
            _dbContext.Execute("delete from FapDict where category=@Category", new DynamicParameters(new { Category = cat }));
            rvm.success = true;
            rvm.msg = "删除成功";
            return Json(rvm);
        }
        /// <summary>
        /// 上传附件
        /// UploadExtraData传来的参数会采用Request.Form["param"]接收
        /// </summary>
        /// <param name="id">用户的ID</param>
        /// <returns></returns>        
        [Route("EmpPhotoImport")]
        public string AttachmentProcess()
        {
            try
            {
                var files = Request.Form.Files;
                if (files != null && files.Count > 0)
                {
                    string fileName = files[0].FileName;
                    string[] names = fileName.Split('_');
                    string empCode = names[0];
                    DynamicParameters param = new DynamicParameters();
                    param.Add("EmpCode", empCode);
                    Fap.Core.Rbac.Model.Employee employee = _dbContext.QueryFirstOrDefaultWhere<Fap.Core.Rbac.Model.Employee>("EmpCode=@EmpCode", param);
                    if (employee == null)
                    {
                        return "0";
                    }

                    if (string.IsNullOrWhiteSpace(employee.EmpPhoto))
                    {
                        employee.EmpPhoto = UUIDUtils.Fid;
                        string sql = "update Employee set EmpPhoto=@EmpPhoto where id=@Id";
                        _dbContext.Execute(sql, new DynamicParameters(new { EmpPhoto = employee.EmpPhoto, Id = employee.Id }));
                    }
                    FapAttachment attachment = new FapAttachment();

                    attachment.Bid = employee.EmpPhoto;
                    attachment.FileName = files[0].FileName;
                    attachment.FileType = files[0].ContentType;
                    string path = SysIO.Path.Combine(Environment.CurrentDirectory, "temp", Guid.NewGuid().ToString());
                    SysIO.FileStream fs = SysIO.File.Create(path);

                    files[0].CopyTo(fs);
                    string attFid = _fileService.UploadFile(fs, attachment);
                }


            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "1";
        }
        [HttpGet("ForceSync/{id}")]
        public JsonResult ForceSync(int id)
        {
            _dbMetadataContext.CreateTable(id);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("RefreshMetadata")]
        public JsonResult RefreshMetadata()
        {
            _platformDomain.ColumnSet.Refresh();
            _platformDomain.TableSet.Refresh();
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("RefreshMultiLanguage")]
        public JsonResult RefreshMultiLanguage()
        {
            _platformDomain.MultiLangSet.Refresh();
            _multiLangService.CreateMultilanguageJsFile();
            return Json(ResponseViewModelUtils.Sueecss());
        }

        [HttpPost("ExportSql")]
        public JsonResult ExportSql(string database, string tableName, string tableCategory, bool isAllTable, bool includCreate, bool includInsert)
        {
            DatabaseDialectEnum dialect = (DatabaseDialectEnum)Enum.Parse(typeof(DatabaseDialectEnum), database);
            //优先级最高
            if (isAllTable)
            {
                var dics = _dbContext.Dictionarys("TableCategory").OrderBy(d => d.SortBy);
                StringBuilder sbALLSql = new StringBuilder();
                foreach (var dic in dics)
                {
                    sbALLSql.AppendLine(_dbMetadataContext.ExportSql(dialect, string.Empty, dic.Code, includCreate, includInsert));
                }
                string allFileName = $"{database}.sql";
                string allFilePath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, allFileName);
                return ZipSql(sbALLSql.ToString(), allFileName, allFilePath);

            }
            if (tableName.IsMissing() && tableCategory.IsMissing())
            {
                return Json(ResponseViewModelUtils.Failure("请选择导出的表或分类"));
            }
            string sql = _dbMetadataContext.ExportSql(dialect, tableName, tableCategory, includCreate, includInsert);
            string fileName = (tableName.IsPresent() ? tableName : tableCategory) + $"{database}.sql";
            string filePath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, fileName);
            if (tableName.IsPresent())
            {
                return Json(new ResponseViewModel { success = true, data = sql });
            }
            return ZipSql(sql, fileName, filePath);
        }

        private JsonResult ZipSql(string sql, string fileName, string filePath)
        {
            SysIO.File.WriteAllText(filePath, sql, Encoding.UTF8);
            ZipHelper zipHelper = new ZipHelper();
            zipHelper.ZipMultiFiles(new[] { filePath }, filePath.Replace(".sql", ".zip"));
            return Json(new ResponseViewModel { success = true, data = $"{FapPlatformConstants.TemporaryFolder}/{fileName.Replace(".sql", ".zip")}" });
        }

        [HttpGet("ExportModelClass/{fid}")]
        public JsonResult ExportModeClass(string fid)
        {
            FapTable table = _dbContext.Get<FapTable>(fid);
            var columns = _dbContext.QueryWhere<FapColumn>($"TableName='{table.TableName}'").Where(c => c.IsDefaultCol == 0);
            string modelClass = _dbMetadataContext.GeneraterModelClass(table, columns);
            return Json(new ResponseViewModel { success = true, data = modelClass });
        }
        [HttpGet("ReportRender/{fid}")]
        public async Task<JsonResult> Render(string fid)
        {
            string path = await _reportService.Render(fid).ConfigureAwait(false);

            ResponseViewModel rvm = new ResponseViewModel { data = path, success = true };
            return Json(rvm);
        }
        [HttpGet("ReportRenderHtml/{fid}")]
        public async Task<JsonResult> RenderHtml(string fid)
        {
            string path = await _reportService.Render(fid).ConfigureAwait(false);
            //string strPath= Path.Combine(Environment.CurrentDirectory, path);
            //ExcelToHTML toHTML = new ExcelToHTML(strPath);
            //string htmlContent = toHTML.ToHtml();
            ResponseViewModel rvm = new ResponseViewModel { data = path, success = true };
            return Json(rvm);
        }
    }
}
