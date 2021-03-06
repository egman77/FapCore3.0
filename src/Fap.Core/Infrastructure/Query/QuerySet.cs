﻿using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Fap.Core.Infrastructure.Query
{
    /// <summary>
    /// 查询设置
    /// </summary>
    public class QuerySet
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 查询列
        /// </summary>
        public string QueryCols { get; set; }
        /// <summary>
        /// 设置可见列，某些情况下需要把一些默认隐藏的列显示出来
        /// </summary>
        public string ReadOnlyCols { get; set; }
       
        /// <summary>
        /// 导出列
        /// </summary>
        public string ExportCols { get; set; }
        /// <summary>
        /// 初始化条件
        /// </summary>
        public string InitWhere { get; set; }

        /// <summary>
        /// 全局条件
        /// </summary>
        public string GlobalWhere { get; set; }
       

        public override string ToString()
        {
            string where = string.Empty;
            if (GlobalWhere.IsPresent())
            {
                where = GlobalWhere;
            }
            if (InitWhere.IsPresent())
            {
                where = where.IsMissing() ? InitWhere : $"{where} and {InitWhere}";
            }
            if (!QueryCols.EqualsWithIgnoreCase("*"))
            {
                var queryColList =QueryCols.Split(',').AsList();
                //id,fid,ts 必选
                if (!queryColList.Contains(FapDbConstants.FAPCOLUMN_FIELD_Id))
                {
                    queryColList.Add(FapDbConstants.FAPCOLUMN_FIELD_Id);
                }
                if (!queryColList.Contains(FapDbConstants.FAPCOLUMN_FIELD_Fid))
                {
                    queryColList.Add(FapDbConstants.FAPCOLUMN_FIELD_Fid);
                }
                if (!queryColList.Contains(FapDbConstants.FAPCOLUMN_FIELD_Ts))
                {
                    queryColList.Add(FapDbConstants.FAPCOLUMN_FIELD_Ts);
                }
                QueryCols = string.Join(',', queryColList);
            }
            return $"select {QueryCols} from {TableName} where {where}";
        }
        /// <summary>
        /// 排序,[字段，排序]
        /// </summary>
        public List<OrderBy> OrderByList { get; set; } = new List<OrderBy>();
        public void AddOrderBy(string field, string direction)
        {
            if (!OrderByList.Exists(o => o.Field.Equals(field, StringComparison.CurrentCultureIgnoreCase)))
            {
                OrderByList.Add(new OrderBy { Field = field, Direction = direction });
            }
        }
        #region 预处理参数
        /// <summary>
        /// 添加预处理参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParameter(string key, string value)
        {

            if (value == null)
            {
                Parameters.Add(new Parameter(key, ""));
                return;
            }

            if (key != null && key.StartsWith("@"))
            {
                key = key.TrimStart('@');
            }
            Parameters.Add(new Parameter(key, value ));

        }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        /// <summary>
        /// 统计设置
        /// </summary>
        private List<StatSet> _statsetlist = new List<StatSet>();
        /// <summary>
        /// 统计设置
        /// </summary>
        public List<StatSet> Statsetlist
        {
            get { return _statsetlist; }
            set { _statsetlist = value; }
        }
        /// <summary>
        /// 添加统计设置
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="field"></param>
        public void AddStatSet(StatSymbolEnum symbol, string field)
        {
            StatSet ss = new StatSet() { Field = field, StatSymbol = symbol };
            Statsetlist.Add(ss);
        }
        #endregion

        #region 处理默认值，用于表单新增的默认值设置


        public List<DefaultValue> DefaultValues { get; set; } = new List<DefaultValue>();

        public void AddDefaultValue(string field, string value)
        {
            DefaultValues.Add(new DefaultValue { Field = field, Value = value });
        }


        #endregion
    }
    public class Parameter
    {
        public Parameter() { }
        public Parameter(string key, string value)
        {
            ParamKey = key;
            ParamValue = value;
        }
        public string ParamKey { get; set; }
        public string ParamValue { get; set; }
    }
    public class OrderBy
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// asc,desc
        /// </summary>
        public string Direction { get; set; }
    }
    public class DefaultValue
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// 统计设置
    /// </summary>
    public class StatSet
    {
        private StatSymbolEnum _StatSymbol = StatSymbolEnum.None;
        /// <summary>
        /// 统计方式
        /// </summary>
        public StatSymbolEnum StatSymbol
        {
            get
            {
                return _StatSymbol;
            }
            set
            {
                _StatSymbol = value;
            }
        }
        /// <summary>
        /// 统计字段
        /// </summary>
        public string Field { get; set; }
    }

    /// <summary>
    /// 统计方式
    /// </summary>
    public enum StatSymbolEnum
    {
        [Description("描述")]
        Description,
        [Description("不统计")]
        None,
        [Description("总数")]
        COUNT,
        [Description("最大值")]
        MAX,
        [Description("最小值")]
        MIN,
        [Description("平均值")]
        AVG,
        [Description("合计")]
        SUM
    }
    public enum QuerySymbolEnum
    {
        [Description("AND")]
        AND,
        [Description("OR")]
        OR,
        [Description("DESC")]
        DESC,
        [Description("ASC")]
        ASC
    }
}
