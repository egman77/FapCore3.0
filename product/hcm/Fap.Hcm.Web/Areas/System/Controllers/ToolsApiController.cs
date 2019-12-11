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

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    [Produces("application/json")]
    [Route("SystemApi/Tools")]
    public class ToolsApiController : FapController
    {
        private IFapFileService _fileService;

        public ToolsApiController(IServiceProvider serviceProvider, IFapFileService fileService) : base(serviceProvider)
        {
            _fileService = fileService;
        }


        /// <summary>
        /// 获取字典分类
        /// </summary>
        /// <returns></returns>
        [Route("DictCategory/")]
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
            ResponseViewModel rvm=new ResponseViewModel();
            if(cat.IsMissing())
            {
                rvm.success=false;
                rvm.msg="字典分类为空，不能删除！";
                return Json(rvm);
            }
            DynamicParameters param=new DynamicParameters();
            param.Add("Category",cat);
            int count= _dbContext.Count("FapColumn", " CtrlType='COMBOBOX' and RefTable=@Category", param);
            if(count>0)
            {
                rvm.success = false;
                rvm.msg = "此编码已经被使用，不能删除！";
                return Json(rvm);
            }
            _dbContext.Execute("delete from FapDict where category=@Category",new DynamicParameters(new { Category = cat }));
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
        [Route("EmpPhotoImport/")]
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
                    Employee employee = _dbContext.QueryFirstOrDefaultWhere<Employee>("EmpCode=@EmpCode", param);
                    if (employee == null)
                    {
                        return "0";
                    }
                   
                    if (string.IsNullOrWhiteSpace(employee.EmpPhoto))
                    {
                        employee.EmpPhoto =UUIDUtils.Fid;
                        string sql = "update Employee set EmpPhoto=@EmpPhoto where id=@Id";
                        _dbContext.Execute(sql,new DynamicParameters(new { EmpPhoto = employee.EmpPhoto ,Id= employee.Id }));
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
    }
}