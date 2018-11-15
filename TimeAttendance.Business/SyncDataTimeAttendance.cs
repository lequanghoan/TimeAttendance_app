using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Model.Repositories;
using TimeAttendance.Utils;

namespace TimeAttendance.Business
{
    public class SyncDataTimeAttendance
    {
        private TimeAttendanceEntities db;

        public bool SyncTimeAttendance(DateTime day)
        {
            try
            {
                db = new TimeAttendanceEntities();
                DateTime dateFrom = DateTimeUtils.ConvertDateFrom(day);
                DateTime dateTo = DateTimeUtils.ConvertDateTo(day);
                var listDataSync = (from a in db.Employee
                                    join b in db.Department on a.DepartmentId equals b.DepartmentId
                                    join c in db.JobTitle on a.JobTitleId equals c.JobTitleId
                                    join d in db.TimeAttendanceLog.Where(r => r.Date >= dateFrom && r.Date <= dateTo) on a.EmployeeId equals d.EmployeeId into ad
                                    from adv in ad.DefaultIfEmpty()
                                    select new
                                    {
                                        a.EmployeeId,
                                        a.Name,
                                        a.Code,
                                        DepartmentName = b.Name,
                                        JobTitleName = c.Name,
                                        Gender = !a.Gender.HasValue ? "Khác" : a.Gender.Value == Constants.Male ? "Nam" : "Nữ",
                                        Date = adv != null ? adv.TimeIn : dateFrom,
                                        TimeIn = adv != null ? adv.TimeIn : null,
                                        TimeOut = adv != null ? adv.TimeOut : null,
                                        Total = adv != null ? adv.Total : null,
                                        LateMinutes = adv != null ? adv.LateMinutes : null,
                                        EarlyMinutes = adv != null ? adv.EarlyMinutes : null,
                                    }).ToList();
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var checkSyncTimeAttendance = db.SyncTimeAttendance.Where(r => r.Date >= dateFrom && r.Date <= dateTo);
                        if (checkSyncTimeAttendance != null && checkSyncTimeAttendance.Count() > 0)
                            db.SyncTimeAttendance.RemoveRange(checkSyncTimeAttendance);

                        List<SyncTimeAttendance> listSync = new List<SyncTimeAttendance>();
                        SyncTimeAttendance model;
                        foreach (var itemSync in listDataSync)
                        {
                            model = new SyncTimeAttendance()
                            {
                                SyncAttendanceId = Guid.NewGuid().ToString(),
                                EmployeeId = itemSync.EmployeeId,
                                EmployeeName = itemSync.Name,
                                Code = itemSync.Code,
                                DepartmentName = itemSync.DepartmentName,
                                JobTitleName = itemSync.JobTitleName,
                                Gender = itemSync.Gender,
                                Date = itemSync.Date.Value,
                                TimeIn = itemSync.TimeIn,
                                TimeOut = itemSync.TimeOut,
                                Total = itemSync.Total,
                                LateMinutes = itemSync.LateMinutes,
                                EarlyMinutes = itemSync.EarlyMinutes,
                            };
                            model.IsLate = false;
                            model.IsEarly = false;
                            model.IsAbsent = false;
                            //Đi muộn
                            if (model.TimeIn != null && model.LateMinutes != null && model.LateMinutes > 0)
                                model.IsLate = true;
                            //Về sớm
                            if (model.TimeOut != null && model.EarlyMinutes != null && model.EarlyMinutes > 0)
                                model.IsEarly = true;
                            //Nghỉ
                            if (model.TimeIn == null && model.TimeOut == null)
                                model.IsAbsent = true;
                            listSync.Add(model);
                        }
                        db.SyncTimeAttendance.AddRange(listSync);
                        db.SaveChanges();
                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SyncTimeAttendanceByDay(DateTime day)
        {
            for (int i = DateTime.Now.Day; i >= 1; i--)
            {
                SyncTimeAttendance(day);
                day = day.AddDays(-1);
            }
            return true;
        }
    }
}
