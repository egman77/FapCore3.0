﻿using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Interface;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Utility;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 请假回写请假统计
    /// </summary>
    [Service]
    public class TmLeavelApplyBillWriteBack : IBillWritebackService
    {
        private readonly ITimeService _timeService;
        private readonly IDbContext _dbContext;
        public TmLeavelApplyBillWriteBack(ITimeService timeService,IDbContext dbContext)
        {
            _timeService = timeService;
            _dbContext = dbContext;
        }
        [Transactional]
        public void Exec(FapDynamicObject billData, FapDynamicObject bizData)
        {
            string empUid= billData.Get("AppEmpUid").ToString();
            string startDateTime = billData.Get("StartTime").ToString();
            string endDateTime = billData.Get("EndTime").ToString();
            var appInfoList = _timeService.LeaveDaysInfo(empUid, startDateTime, endDateTime);
            string billUid = billData.Get("Fid").ToString();
            string leaveType = billData.Get("LeaveType").ToString();
            IList<TmLeaveStat> stats = new List<TmLeaveStat>();
            foreach (var appinfo in appInfoList)
            {
                TmLeaveStat lstat = new TmLeaveStat
                {
                    EmpUid = empUid,
                    BillUid = billUid,
                    WorkDate = appinfo.ApplyDate,
                    LeaveTypeUid=leaveType,
                    LeaveDays = appinfo.Days,
                    LeaveHours = appinfo.Hours
                };
                stats.Add(lstat);
            }
            _dbContext.InsertBatchSql(stats);
            //扣减年假
            if (leaveType == "Annaul")
            {
                var annualLeave = _dbContext.QueryFirstOrDefault<TmAnnualLeave>("select * from TmAnnualLeave where EmpUid=@EmpUid and Annual=@Year",
                    new Dapper.DynamicParameters(new { EmpUid = empUid, Year = DateTimeUtils.ToDateTime(startDateTime).Year }));
                if (annualLeave != null)
                {
                    annualLeave.UsedNum += (billData.Get("IntervalDay").ToDouble());
                    annualLeave.RemainderNum = annualLeave.CurrRealNum - annualLeave.UsedNum;
                    _dbContext.Update(annualLeave);
                }
            
            }

        }
    }
}