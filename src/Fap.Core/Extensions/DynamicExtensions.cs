﻿using Fap.Core.Metadata;
using Fap.Core.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Extensions
{
    public static class DynamicExtensions
    {
        /// <summary>
        /// 序列化json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="firstLower">首字母小写</param>
        /// <returns></returns>
        public static string ToJsonIgnoreNullValue(this object obj, bool firstLower = true)
        {
            if (firstLower)
            {
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() };
                return JsonConvert.SerializeObject(obj, Formatting.Indented, jSetting);
            }
            else
            {
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                return JsonConvert.SerializeObject(obj, Formatting.Indented, jSetting);
            }

        }
        public static string ToJson(this object obj, bool firstLower = true)
        {
            if (firstLower)
            {
                var jSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                return JsonConvert.SerializeObject(obj, Formatting.Indented, jSetting);
            }
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将Dapper查询的结果转换为动态对象
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ToFapDynamicObjectList(this IEnumerable<dynamic> dataList, string tableName)
        {
            //组装成动态对象集合
            List<FapDynamicObject> result = new List<FapDynamicObject>();
            if (dataList != null)
            {
                foreach (dynamic item in dataList)
                {
                    var dr = item as IDictionary<string, object>;
                    if (dr != null)
                    {
                        result.Add(dr.ToFapDynamicObject(tableName));
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 将Dapper动态类型对象转成Fap动态类型对象
        /// </summary>
        /// <param name="dapperRow">dapperRow转换的IDictionary</param>
        /// <returns></returns>
        public static dynamic ToFapDynamicObject(this IDictionary<string, object> dynamicData, string tableName)
        {
            dynamic obj = new FapDynamicObject(tableName);
            List<string> keyList = new List<string>(dynamicData.Keys);
            foreach (var key in keyList)
            {
                obj.Add(key, dynamicData[key]);
            }
            return obj;
        }


    }
}

