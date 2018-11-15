using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Model;
using TimeAttendance.Model.Repositories;
using TimeAttendance.Utils;
using System.Web.Hosting;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using TimeAttendance.Storage;
using System.Globalization;
using TimeAttendance.Model.CacheModel;
using TimeAttendance.Caching;
using TimeAttendance.Model.Entities;

namespace TimeAttendance.Business
{
    public class FaceHelperFuntionBusiness
    {

        private TimeAttendanceEntities db;
        public string _connStr;
        public string _RedisConnection;
        string itemPersonId = "";
        public FaceHelperFuntionBusiness(ConnectionModel connectionModel)
        {
            _RedisConnection = connectionModel.RedisConnection;
            _connStr = connectionModel.connStr;
            if (db == null)
            {
                db = new TimeAttendanceEntities(_connStr);
            }
        }

        private TimeAttendanceLog modelTimeAttendanceLog;
        private AttendanceLog modelAttendanceLog;
        private const int FaceImageBoxPaddingPercentage = 50;
        Employee emp;
        public void LogTimeAttendanceFuntion(IEnumerable<IdentifyResult> listIdentifiedPerson, DateTime date, string imageLink)
        {
            try
            {
                if (db == null)
                {
                    db = new TimeAttendanceEntities(_connStr);
                }
                List<TimeAttendanceLog> listTimeAttendanceLog = new List<TimeAttendanceLog>();
                var logDateTo = DateTimeUtils.ConvertDateFrom(date);
                var listLogNow = db.TimeAttendanceLog.Where(r => r.Date == logDateTo.Date);
                TimeSpan timeSpan;
                TimeSpan timeSpan2;
                int sophut;
                //Lấy nhân viên có độ chính xác >= mức độ chính xác quy định
                foreach (var item in listIdentifiedPerson)
                {
                    if (item.Candidates.Length > 0)
                    {
                        itemPersonId = item.Candidates[0].PersonId.ToString();
                        emp = db.Employee.FirstOrDefault(u => u.FaceId.Equals(itemPersonId));
                        if (emp != null)
                        {
                            modelTimeAttendanceLog = listLogNow.FirstOrDefault(r => r.EmployeeId.Equals(emp.EmployeeId));
                            if (modelTimeAttendanceLog != null)
                            {
                                timeSpan = (date - modelTimeAttendanceLog.TimeIn.Value);
                                modelTimeAttendanceLog.TimeOut = date;
                                modelTimeAttendanceLog.ImageOut = imageLink;
                                modelTimeAttendanceLog.Total = (int)(timeSpan.TotalHours * 60);
                                timeSpan2 = (date - TimeAttendanceStatic.EndTime.Value);
                                modelTimeAttendanceLog.EarlyMinutes = date < TimeAttendanceStatic.EndTime.Value ? (Math.Abs((timeSpan2.Hours * 60)) + Math.Abs(timeSpan2.Minutes)) : 0;
                                modelTimeAttendanceLog.ImageFaceOut = imageLink;
                            }
                            else
                            {
                                timeSpan = (date - TimeAttendanceStatic.StartTime.Value);
                                sophut = (Math.Abs((timeSpan.Hours * 60)) + Math.Abs(timeSpan.Minutes));
                                modelTimeAttendanceLog = new TimeAttendanceLog()
                                {
                                    TimeAttendanceLogId = Guid.NewGuid().ToString(),
                                    EmployeeId = emp.EmployeeId,
                                    Date = logDateTo,
                                    TimeIn = date,
                                    ImageIn = imageLink,
                                    LateMinutes = date < TimeAttendanceStatic.StartTime.Value ? 0 : Math.Abs(sophut),
                                    ImageFaceIn = imageLink
                                };
                                var isExisted = (from r in db.TimeAttendanceLog.AsNoTracking()
                                                 where r.Date == logDateTo.Date && r.EmployeeId.Equals(emp.EmployeeId)
                                                select r.TimeAttendanceLogId).Count() > 0;
                                if (!isExisted)
                                {
                                    listTimeAttendanceLog.Add(modelTimeAttendanceLog);
                                }
                            }
                        }
                    }
                }
                if (listTimeAttendanceLog.Count > 0)
                {
                    db.TimeAttendanceLog.AddRange(listTimeAttendanceLog);
                }
                db.SaveChanges();

            }
            catch (Exception ex1)
            {
            }
        }
        public List<InfoNotificationResult> LogAttendanceFuntion(IEnumerable<IdentifyResult> listIdentifiedPerson, DateTime date, string imageLink, string CameraIPAdress)
        {
            string JobTitle = "";
            string DepartmentName = "";
            string idGuid = "";
            List<InfoNotificationResult> lstRs = new List<InfoNotificationResult>();
            InfoNotificationResult itemLog;
            if (db == null)
            {
                db = new TimeAttendanceEntities(_connStr);
            }
            try
            {
                List<AttendanceLog> listTimeAttendanceLog = new List<AttendanceLog>();
                var logDateTo = date;
                TimeSpan timeSpan;
                foreach (var item in listIdentifiedPerson)
                {
                    itemPersonId = "";
                    if (item.Candidates.Length > 0)
                    {
                        try
                        {

                            itemPersonId = item.Candidates[0].PersonId.ToString();
                        }
                        catch (Exception)
                        { }
                    }

                    emp = db.Employee.FirstOrDefault(u => u.FaceId.Equals(itemPersonId));
                    if (emp != null)
                    {
                        #region[lấy phòng và chức vụ]
                        try
                        {
                            JobTitle = db.JobTitle.FirstOrDefault(u => u.JobTitleId.Equals(emp.JobTitleId)).Name;
                        }
                        catch (Exception)
                        { JobTitle = "Unknown"; }
                        try
                        {
                            DepartmentName = db.Department.FirstOrDefault(u => u.DepartmentId.Equals(emp.DepartmentId)).Name;
                        }
                        catch (Exception)
                        { DepartmentName = "Unknown"; }
                    }
                    else
                    {
                        DepartmentName = "Unknown";
                        JobTitle = "Unknown";
                    }

                    #endregion

                    idGuid = Guid.NewGuid().ToString();
                    itemLog = new InfoNotificationResult();
                    itemLog.AttendanceLogId = idGuid;
                    itemLog.Code = emp != null ? emp.Code : "Unknown";
                    itemLog.Name = emp != null ? emp.Name : "Unknown";
                    itemLog.DepartmentName = DepartmentName;
                    itemLog.JobTitle = JobTitle;
                    itemLog.ImageLink = imageLink;
                    itemLog.DateLog = logDateTo.ToString("dd/MM/yyyy");
                    itemLog.TimeLog = logDateTo.ToString("HH:mm:ss");
                    itemLog.Confidence = ((item.Candidates.Length > 0) ? ((float)(item.Candidates[0].Confidence)) : 0);
                    lstRs.Add(itemLog);

                    timeSpan = (date - TimeAttendanceStatic.StartTime.Value);
                    modelAttendanceLog = new AttendanceLog()
                    {
                        AttendanceLogId = idGuid,
                        EmployeeId = emp != null ? emp.EmployeeId : "Unknown",
                        Date = logDateTo,
                        ImageLink = imageLink,
                        CameraIPAdress = CameraIPAdress,
                        Confidence = (item.Candidates.Length > 0) ? item.Candidates[0].Confidence : 0,
                        FaceCount = listIdentifiedPerson.Count(),
                        ImageFace = imageLink,
                    };
                    listTimeAttendanceLog.Add(modelAttendanceLog);


                }
                if (listTimeAttendanceLog.Count > 0)
                    db.AttendanceLog.AddRange(listTimeAttendanceLog);
                db.SaveChanges();
            }
            catch (Exception ex)
            { }
            return lstRs;

        }

        public void AddOrUpdateCacheDensityFuntion(IEnumerable<IdentifyResult> listIdentifiedPerson, DateTime DateNow)
        {
            //  db = new TimeAttendanceEntities(_connStr);
            //Ngày hiện tại
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = dfi.Calendar;
            //Tuần hiện tại
            var WeekNow = calendar.GetWeekOfYear(DateNow, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            //Ngày đầu của tuần hiện tại
            DateTime firstDayOfWeekNow = DateTimeUtils.FirstDayOfWeek(DateNow);
            //Ngày cuối của tuần hiện tại
            DateTime lastDayOfWeekNow = firstDayOfWeekNow.AddDays(6);

            var DateFrom_ToKey = "AT:Density:" + firstDayOfWeekNow.ToString("ddMMyyyy") + "_" + lastDayOfWeekNow.ToString("ddMMyyyy");
            var redis = RedisService<DensityCache>.GetInstance(_RedisConnection);
            DensityCache cacheModel;
            cacheModel = redis.Get<DensityCache>(DateFrom_ToKey);
            if (cacheModel != null)
            {
                //nếu có rồi thì cập nhật lại
                var dateKey = DateNow.ToString("dd/MM/yyyy/") + DateNow.Hour;
                var rowUpdate = cacheModel.ListResult.FirstOrDefault(u => u.DateKey.Equals(dateKey));
                if (rowUpdate != null)
                {
                    rowUpdate.VehicleInStatistic = rowUpdate.VehicleInStatistic + listIdentifiedPerson.Count();
                }
                redis.Replace<DensityCache>(DateFrom_ToKey, cacheModel);

            }
            else
            {
                //chưa thì mình khởi tạo cache cho tuần mới
                List<MCRP3100HeatMap> listMCRP3100HeatMap = new List<MCRP3100HeatMap>();
                DateTime monday;
                string dateToList = string.Empty;
                List<string> listDay = new List<string>();
                string[] listTime = { "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00" };
                var dateCount = lastDayOfWeekNow - firstDayOfWeekNow;
                for (int i = 0; i < dateCount.Days + 1; i++)
                {
                    monday = firstDayOfWeekNow.AddDays(i);
                    dateToList = monday.Date.ToString("dd/MM/yyyy");
                    listDay.Add(dateToList);
                    for (int j = 1; j < 25; j++)
                    {
                        listMCRP3100HeatMap.Add(new MCRP3100HeatMap
                        {
                            x = j - 1,
                            y = i,
                            VehicleInStatistic = 0,
                            DateKey = dateToList + "/" + (j - 1)
                        });

                    }
                }
                var dateKey = DateNow.ToString("dd/MM/yyyy/") + DateNow.Hour;
                var rowUpdate = listMCRP3100HeatMap.FirstOrDefault(u => u.DateKey.Equals(dateKey));
                if (rowUpdate != null)
                {
                    rowUpdate.VehicleInStatistic = listIdentifiedPerson.Count();
                }

                //add vào cache
                cacheModel = new DensityCache();
                cacheModel.ListResult = listMCRP3100HeatMap;
                cacheModel.listDay = listDay;
                cacheModel.listTime = listTime.ToList();
                redis.Add(DateFrom_ToKey, cacheModel);

            }
        }

        public void AddOrUpdateCacheColectionFuntion()
        {
            // db = new TimeAttendanceEntities(_connStr);
            DashboardResultModel modelResult = new DashboardResultModel();
            try
            {
                var lstEmp = db.Employee.ToList();
                var lstEmpId = lstEmp.Select(r => r.EmployeeId);
                modelResult.TotalEmployee = lstEmpId.Count();

                //Ngày hiện tại
                modelResult.DateNow = DateTime.Now;
                //Ngày hôm qua
                modelResult.DateBefore = modelResult.DateNow.AddDays(-1);
                modelResult.DayNow = modelResult.DateNow.ToString("dd/MM/yyyy");

                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                Calendar calendar = dfi.Calendar;
                //Tuần hiện tại
                modelResult.WeekNow = calendar.GetWeekOfYear(modelResult.DateNow, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                //Ngày đầu của tuần hiện tại
                DateTime firstDayOfWeekNow = DateTimeUtils.FirstDayOfWeek(modelResult.DateNow);
                //Ngày cuối của tuần hiện tại
                DateTime lastDayOfWeekNow = firstDayOfWeekNow.AddDays(6);

                //Ngày đầu của tuần trước
                DateTime firstDayOfWeekBefore = firstDayOfWeekNow.AddDays(-7);
                //Ngày cuối của tuần trước
                DateTime lastDayOfWeekBefore = firstDayOfWeekBefore.AddDays(6);
                //Tuần trước
                modelResult.WeekBefore = calendar.GetWeekOfYear(firstDayOfWeekBefore, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

                //Tháng hiện tại
                modelResult.MonthNow = modelResult.DateNow.Month;
                //Ngày đầu của tháng hiện tại
                DateTime firstDayOfMonthNow = new DateTime(modelResult.DateNow.Year, modelResult.MonthNow, 1);
                //Ngày cuối của tháng hiện tại
                DateTime lastDayOfMonthNow = firstDayOfMonthNow.AddMonths(1).AddDays(-1);
                //Tháng trước
                modelResult.MonthBefore = modelResult.DateNow.AddMonths(-1).Month;
                //Ngày đầu của tháng trước
                DateTime firstDayOfMonthBefore = new DateTime(modelResult.DateNow.AddMonths(-1).Year, modelResult.DateNow.AddMonths(-1).Month, 1);
                //Ngày cuối của tháng trước
                DateTime lastDayOfMonthBefore = firstDayOfMonthBefore.AddMonths(1).AddDays(-1);
                //Quý hiện tại
                modelResult.QuarterNow = ((modelResult.DateNow.Month + 2) / 3);

                //Ngày đầu của quý hiện tại
                DateTime firstDayOfQuarterNow = new DateTime(modelResult.DateNow.Year, (modelResult.QuarterNow - 1) * 3 + 1, 1);
                //Ngày cuối của quý hiện tại
                DateTime lastDayOfQuarterNow = firstDayOfQuarterNow.AddMonths(3).AddDays(-1);

                //Lấy quý trước
                DateTime dateBeforeOfQuarterNow = firstDayOfQuarterNow.AddMonths(-1);
                modelResult.QuarterBefore = ((dateBeforeOfQuarterNow.Month + 2) / 3);
                //Ngày đầu của quý trước
                DateTime firstDayOfQuarterBefore = new DateTime(dateBeforeOfQuarterNow.Year, (modelResult.QuarterBefore - 1) * 3 + 1, 1);
                //Ngày cuối của quý trước
                DateTime lastDayOfQuarterBefore = firstDayOfQuarterBefore.AddMonths(3).AddDays(-1);

                DateTime dateFrom;
                DateTime dateTo;

                //lấy tổng dữ liệu trong quý
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfQuarterNow);
                dateTo = DateTimeUtils.ConvertDateTo(lastDayOfQuarterNow);
                var listQuarterNow = db.TimeAttendanceLog.Where(u => u.Date >= dateFrom && u.Date <= dateTo).ToList();

                //lấy dữ liệu trong ngày
                dateFrom = DateTimeUtils.ConvertDateFrom(modelResult.DateNow);
                dateTo = DateTimeUtils.ConvertDateTo(modelResult.DateNow);
                var listDayNow = listQuarterNow.Where(u => u.Date >= dateFrom && u.Date <= dateTo).ToList();

                //lấy dữ liệu trong tuần
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfWeekNow);
                dateTo = DateTimeUtils.ConvertDateTo(lastDayOfWeekNow);
                var listWeekNow = listQuarterNow.Where(u => u.Date >= dateFrom && u.Date <= dateTo).ToList();

                //lấy dữ liệu trong tháng
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfMonthNow);
                dateTo = DateTimeUtils.ConvertDateTo(lastDayOfMonthNow);
                var listMonthNow = listQuarterNow.Where(u => u.Date >= dateFrom && u.Date <= dateTo).ToList();

                #region  //Thống kê chấm công ngày hiện tại và ngày trước
                //tinh toán
                modelResult.ListDataDay = new List<DataResultModel>();
                modelResult.ListDataDay.Add(new DataResultModel()
                {
                    Name = "Ngày " + modelResult.DateNow.ToString("dd/MM/yyyy") + "",
                    TotalLate = listDayNow.Where(u => u.LateMinutes.HasValue && u.LateMinutes > 0).Count(),
                    TotalEarly = listDayNow.Where(u => u.EarlyMinutes.HasValue && u.EarlyMinutes > 0).Count(),
                    TotalAbsent = listDayNow.Where(u => !lstEmpId.Contains(u.EmployeeId)).Count()
                });

                var dataDayNow = modelResult.ListDataDay.First();
                modelResult.LatePercent = Math.Round((double)(dataDayNow.TotalLate / modelResult.TotalEmployee) * 100, 2);
                modelResult.EarlyPercent = Math.Round((double)(dataDayNow.TotalEarly / modelResult.TotalEmployee) * 100, 2);
                modelResult.AbsentPercent = Math.Round((double)(dataDayNow.TotalAbsent / modelResult.TotalEmployee) * 100, 2);

                #region[lấy cache ngày]

                var DateNowKey = "AT:DayHome:" + modelResult.DateNow.ToString("ddMMyyyy");
                var redisDayHomeCache = RedisService<DayHomeCache>.GetInstance(_RedisConnection);
                DayHomeCache DayHomeCache;
                DayHomeCache = redisDayHomeCache.Get<DayHomeCache>(DateNowKey);
                if (DayHomeCache != null)
                {
                    DayHomeCache.ListDataDay = modelResult.ListDataDay;
                    DayHomeCache.LatePercent = modelResult.LatePercent;
                    DayHomeCache.EarlyPercent = modelResult.EarlyPercent;
                    DayHomeCache.AbsentPercent = modelResult.AbsentPercent;
                    //add vào cache
                    redisDayHomeCache.Replace(DateNowKey, DayHomeCache);
                }
                else
                {

                    DayHomeCache = new DayHomeCache();

                    DayHomeCache.ListDataDay = modelResult.ListDataDay;
                    DayHomeCache.LatePercent = modelResult.LatePercent;
                    DayHomeCache.EarlyPercent = modelResult.EarlyPercent;
                    DayHomeCache.AbsentPercent = modelResult.AbsentPercent;

                    //add vào cache
                    redisDayHomeCache.Add(DateNowKey, DayHomeCache);
                }
                #endregion

                #endregion

                #region  //Thống kê chấm công tuần hiện tại và tuần trước
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfWeekNow);
                dateTo = DateTimeUtils.ConvertDateTo(lastDayOfWeekNow);

                //tính toán 
                modelResult.ListDataWeek = new List<DataResultModel>();
                modelResult.ListDataWeek.Add(new DataResultModel()
                {
                    Name = "Tuần " + modelResult.WeekNow + " Năm " + firstDayOfWeekNow.ToString("yyyy"),
                    TotalLate = listWeekNow.Where(u => u.LateMinutes.HasValue && u.LateMinutes > 0).Count(),
                    TotalEarly = listWeekNow.Where(u => u.EarlyMinutes.HasValue && u.EarlyMinutes > 0).Count(),
                    TotalAbsent = listWeekNow.Where(u => !lstEmpId.Contains(u.EmployeeId)).Count()
                });
                modelResult.ListLateTopInWeek = (from b in lstEmp
                                                 select new DataTopModel
                                                 {
                                                     Code = b.Code,
                                                     EmployeeName = b.Name,
                                                     Total = (int)(from x in listWeekNow
                                                                   where x.EmployeeId.Equals(b.EmployeeId) && x.LateMinutes.HasValue
                                                                   select x.TimeAttendanceLogId).Count(),
                                                     TotalTime = (int)(from x in listWeekNow
                                                                       where x.EmployeeId.Equals(b.EmployeeId) && x.LateMinutes.HasValue
                                                                       select x.LateMinutes).Sum(),

                                                 }).Where(r => r.Total > 0).OrderByDescending(r => r.Total).ThenBy(r => r.TotalTime).Take(20).ToList();
                modelResult.ListEarlyTopInWeek = (from b in lstEmp
                                                  select new DataTopModel
                                                  {
                                                      Code = b.Code,
                                                      EmployeeName = b.Name,
                                                      Total = (int)(from x in listWeekNow
                                                                    where x.EmployeeId.Equals(b.EmployeeId) && x.EarlyMinutes.HasValue
                                                                    select x.TimeAttendanceLogId).Count(),
                                                      TotalTime = (int)(from x in listWeekNow
                                                                        where x.EmployeeId.Equals(b.EmployeeId) && x.EarlyMinutes.HasValue
                                                                        select x.EarlyMinutes).Sum(),

                                                  }).Where(r => r.Total > 0).OrderByDescending(r => r.Total).ThenBy(r => r.TotalTime).Take(20).ToList();
                //Dữ liệu biểu đồ
                modelResult.DataChartWeek = new DataChartModel();
                for (DateTime dateWeek = firstDayOfWeekNow; dateWeek <= lastDayOfWeekNow;)
                {
                    dateFrom = DateTimeUtils.ConvertDateFrom(dateWeek);
                    dateTo = DateTimeUtils.ConvertDateTo(dateWeek);
                    var listDataChart = listWeekNow.Where(r => r.Date >= dateFrom && r.Date <= dateTo).ToList();

                    modelResult.DataChartWeek.ListTotalLate.Add(listDataChart.Where(r => r.LateMinutes.HasValue && r.LateMinutes > 0).Count());
                    modelResult.DataChartWeek.ListTotalEarly.Add(listDataChart.Where(r => r.EarlyMinutes.HasValue && r.EarlyMinutes > 0).Count());
                    modelResult.DataChartWeek.ListTotalAbsent.Add(listDataChart.Where(r => !lstEmpId.Contains(r.EmployeeId)).Count());
                    dateWeek = dateWeek.AddDays(1);
                }


                #region[lấy cache ngày]
                var DateFrom_ToKeyW = "AT:WeekHome:" + firstDayOfWeekNow.ToString("ddMMyyyy") + "_" + lastDayOfWeekNow.ToString("ddMMyyyy");
                var redisWeekHomeCache = RedisService<WeekHomeCache>.GetInstance(_RedisConnection);
                WeekHomeCache WeekHomeCache;
                WeekHomeCache = redisWeekHomeCache.Get<WeekHomeCache>(DateFrom_ToKeyW);
                if (WeekHomeCache != null)
                {
                    WeekHomeCache.ListDataWeek = modelResult.ListDataWeek;
                    WeekHomeCache.ListLateTopInWeek = modelResult.ListLateTopInWeek;
                    WeekHomeCache.ListEarlyTopInWeek = modelResult.ListEarlyTopInWeek;
                    WeekHomeCache.DataChartWeek = modelResult.DataChartWeek;

                    //add vào cache
                    redisWeekHomeCache.Replace(DateFrom_ToKeyW, WeekHomeCache);
                }
                else
                {

                    WeekHomeCache = new WeekHomeCache();
                    WeekHomeCache.ListDataWeek = modelResult.ListDataWeek;
                    WeekHomeCache.ListLateTopInWeek = modelResult.ListLateTopInWeek;
                    WeekHomeCache.ListEarlyTopInWeek = modelResult.ListEarlyTopInWeek;
                    WeekHomeCache.DataChartWeek = modelResult.DataChartWeek;

                    //add vào cache
                    redisWeekHomeCache.Add(DateFrom_ToKeyW, WeekHomeCache);
                }
                #endregion
                #endregion

                #region  //Thống kê chấm công tháng hiện tại và tháng trước
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfMonthNow);
                dateTo = DateTimeUtils.ConvertDateTo(lastDayOfMonthNow);
                #region //tính toán 
                modelResult.ListDataMonth = new List<DataResultModel>();
                modelResult.ListDataMonth.Add(new DataResultModel()
                {
                    Name = "Tháng " + modelResult.MonthNow + " Năm " + firstDayOfMonthNow.ToString("yyyy"),
                    TotalLate = listMonthNow.Where(u => u.LateMinutes.HasValue && u.LateMinutes > 0).Count(),
                    TotalEarly = listMonthNow.Where(u => u.EarlyMinutes.HasValue && u.EarlyMinutes > 0).Count(),
                    TotalAbsent = listMonthNow.Where(u => !lstEmpId.Contains(u.EmployeeId)).Count()
                });
                modelResult.ListLateTopInMonth = (from b in lstEmp
                                                  select new DataTopModel
                                                  {
                                                      Code = b.Code,
                                                      EmployeeName = b.Name,
                                                      Total = (int)(from x in listMonthNow
                                                                    where x.EmployeeId.Equals(b.EmployeeId) && x.LateMinutes.HasValue
                                                                    select x.TimeAttendanceLogId).Count(),
                                                      TotalTime = (int)(from x in listWeekNow
                                                                        where x.EmployeeId.Equals(b.EmployeeId) && x.LateMinutes.HasValue
                                                                        select x.LateMinutes).Sum(),

                                                  }).Where(r => r.Total > 0).OrderByDescending(r => r.Total).ThenBy(r => r.TotalTime).Take(20).ToList();
                modelResult.ListEarlyTopInMonth = (from b in lstEmp
                                                   select new DataTopModel
                                                   {
                                                       Code = b.Code,
                                                       EmployeeName = b.Name,
                                                       Total = (int)(from x in listMonthNow
                                                                     where x.EmployeeId.Equals(b.EmployeeId) && x.EarlyMinutes.HasValue
                                                                     select x.TimeAttendanceLogId).Count(),
                                                       TotalTime = (int)(from x in listWeekNow
                                                                         where x.EmployeeId.Equals(b.EmployeeId) && x.EarlyMinutes.HasValue
                                                                         select x.EarlyMinutes).Sum(),

                                                   }).Where(r => r.Total > 0).OrderByDescending(r => r.Total).ThenBy(r => r.TotalTime).Take(20).ToList();

                ////Dữ liệu biểu đồ
                modelResult.DataChartMonth = new DataChartModel();
                for (DateTime dateMonth = firstDayOfMonthNow; dateMonth <= lastDayOfMonthNow;)
                {
                    dateFrom = DateTimeUtils.ConvertDateFrom(dateMonth);
                    dateTo = DateTimeUtils.ConvertDateTo(dateMonth);
                    var listDataChart = listMonthNow.Where(r => r.Date >= dateFrom && r.Date <= dateTo).ToList();

                    modelResult.DataChartMonth.ListCategories.Add(dateMonth.Day.ToString());
                    modelResult.DataChartMonth.ListTotalLate.Add(listDataChart.Where(r => r.LateMinutes.HasValue && r.LateMinutes > 0).Count());
                    modelResult.DataChartMonth.ListTotalEarly.Add(listDataChart.Where(r => r.EarlyMinutes.HasValue && r.EarlyMinutes > 0).Count());
                    modelResult.DataChartMonth.ListTotalAbsent.Add(listDataChart.Where(r => !lstEmpId.Contains(r.EmployeeId)).Count());
                    dateMonth = dateMonth.AddDays(1);
                }
                #endregion
                #region[lấy cache ngày]
                var DateFrom_ToKeyM = "AT:MonthHome:" + firstDayOfMonthNow.ToString("ddMMyyyy") + "_" + lastDayOfMonthNow.ToString("ddMMyyyy");
                var redisMonthHomeCache = RedisService<MonthHomeCache>.GetInstance(_RedisConnection);
                MonthHomeCache MonthHomeCache;
                MonthHomeCache = redisMonthHomeCache.Get<MonthHomeCache>(DateFrom_ToKeyM);
                if (MonthHomeCache != null)
                {
                    MonthHomeCache.ListDataMonth = modelResult.ListDataMonth;
                    MonthHomeCache.ListLateTopInMonth = modelResult.ListLateTopInMonth;
                    MonthHomeCache.ListEarlyTopInMonth = modelResult.ListEarlyTopInMonth;
                    MonthHomeCache.DataChartMonth = modelResult.DataChartMonth;

                    redisMonthHomeCache.Replace(DateFrom_ToKeyM, MonthHomeCache);
                }
                else
                {
                    MonthHomeCache = new MonthHomeCache();
                    MonthHomeCache.ListDataMonth = modelResult.ListDataMonth;
                    MonthHomeCache.ListLateTopInMonth = modelResult.ListLateTopInMonth;
                    MonthHomeCache.ListEarlyTopInMonth = modelResult.ListEarlyTopInMonth;
                    MonthHomeCache.DataChartMonth = modelResult.DataChartMonth;

                    redisMonthHomeCache.Add(DateFrom_ToKeyM, MonthHomeCache);
                }
                #endregion


                #endregion

                #region  //Thống kê chấm công quý hiện tại và quý trước
                #region  //tính toán
                modelResult.ListDataQuarter = new List<DataResultModel>();
                modelResult.ListDataQuarter.Add(new DataResultModel()
                {
                    Name = "Quý " + modelResult.QuarterNow + " Năm " + firstDayOfQuarterNow.ToString("yyyy"),
                    TotalLate = listQuarterNow.Where(r => r.LateMinutes.HasValue && r.LateMinutes > 0).Count(),
                    TotalEarly = listQuarterNow.Where(r => r.EarlyMinutes.HasValue && r.EarlyMinutes > 0).Count(),
                    TotalAbsent = listQuarterNow.Where(r => !lstEmpId.Contains(r.EmployeeId)).Count()
                });
                ////Dữ liệu top
                modelResult.ListLateTopInQuarter = (from b in lstEmp
                                                    select new DataTopModel
                                                    {
                                                        Code = b.Code,
                                                        EmployeeName = b.Name,
                                                        Total = (int)(from x in listQuarterNow
                                                                      where x.EmployeeId.Equals(b.EmployeeId) && x.LateMinutes.HasValue
                                                                      select x.TimeAttendanceLogId).Count(),
                                                        TotalTime = (int)(from x in listWeekNow
                                                                          where x.EmployeeId.Equals(b.EmployeeId) && x.LateMinutes.HasValue
                                                                          select x.LateMinutes).Sum(),

                                                    }).Where(r => r.Total > 0).OrderByDescending(r => r.Total).ThenBy(r => r.TotalTime).Take(20).ToList();
                modelResult.ListEarlyTopInQuarter = (from b in lstEmp
                                                     select new DataTopModel
                                                     {
                                                         Code = b.Code,
                                                         EmployeeName = b.Name,
                                                         Total = (int)(from x in listQuarterNow
                                                                       where x.EmployeeId.Equals(b.EmployeeId) && x.EarlyMinutes.HasValue
                                                                       select x.TimeAttendanceLogId).Count(),
                                                         TotalTime = (int)(from x in listWeekNow
                                                                           where x.EmployeeId.Equals(b.EmployeeId) && x.EarlyMinutes.HasValue
                                                                           select x.EarlyMinutes).Sum(),

                                                     }).Where(r => r.Total > 0).OrderByDescending(r => r.Total).ThenBy(r => r.TotalTime).Take(20).ToList();

                modelResult.DataChartQuarter = new DataChartModel();
                //Tháng thứ 1 của quý
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfQuarterNow);
                dateTo = DateTimeUtils.ConvertDateTo(firstDayOfQuarterNow.AddMonths(1).AddDays(1));
                var listDataChartQuarter = listQuarterNow.Where(r => r.Date >= dateFrom && r.Date <= dateTo).ToList();
                modelResult.DataChartQuarter.ListCategories.Add("Tháng " + firstDayOfQuarterNow.Month.ToString());
                modelResult.DataChartQuarter.ListTotalLate.Add(listDataChartQuarter.Where(r => r.LateMinutes.HasValue && r.LateMinutes > 0).Count());
                modelResult.DataChartQuarter.ListTotalEarly.Add(listDataChartQuarter.Where(r => r.EarlyMinutes.HasValue && r.EarlyMinutes > 0).Count());
                modelResult.DataChartQuarter.ListTotalAbsent.Add(listDataChartQuarter.Where(r => !lstEmpId.Contains(r.EmployeeId)).Count());

                ////Tháng thứ 2 của quý
                DateTime firstDayOfQuarterNext = firstDayOfQuarterNow.AddMonths(1);
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfQuarterNext);
                dateTo = DateTimeUtils.ConvertDateTo(firstDayOfQuarterNext.AddMonths(1).AddDays(1));
                listDataChartQuarter = listQuarterNow.Where(r => r.Date >= dateFrom && r.Date <= dateTo).ToList();
                modelResult.DataChartQuarter.ListCategories.Add("Tháng " + firstDayOfQuarterNext.Month.ToString());
                modelResult.DataChartQuarter.ListTotalLate.Add(listDataChartQuarter.Where(r => r.LateMinutes.HasValue && r.LateMinutes > 0).Count());
                modelResult.DataChartQuarter.ListTotalEarly.Add(listDataChartQuarter.Where(r => r.EarlyMinutes.HasValue && r.EarlyMinutes > 0).Count());
                modelResult.DataChartQuarter.ListTotalAbsent.Add(listDataChartQuarter.Where(r => !lstEmpId.Contains(r.EmployeeId)).Count());

                ////Tháng thứ 3 của quý
                firstDayOfQuarterNext = firstDayOfQuarterNow.AddMonths(2);
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfQuarterNext);
                dateTo = DateTimeUtils.ConvertDateTo(firstDayOfQuarterNext.AddMonths(1).AddDays(1));
                listDataChartQuarter = listQuarterNow.Where(r => r.Date >= dateFrom && r.Date <= dateTo).ToList();
                modelResult.DataChartQuarter.ListCategories.Add("Tháng " + firstDayOfQuarterNext.Month.ToString());
                modelResult.DataChartQuarter.ListTotalLate.Add(listDataChartQuarter.Where(r => r.LateMinutes.HasValue && r.LateMinutes > 0).Count());
                modelResult.DataChartQuarter.ListTotalEarly.Add(listDataChartQuarter.Where(r => r.EarlyMinutes.HasValue && r.EarlyMinutes > 0).Count());
                modelResult.DataChartQuarter.ListTotalAbsent.Add(listDataChartQuarter.Where(r => !lstEmpId.Contains(r.EmployeeId)).Count());
                #endregion

                #region[lấy cache ngày]
                var DateFrom_ToKeyQ = "AT:QuarterHome:" + firstDayOfQuarterNow.ToString("ddMMyyyy") + "_" + lastDayOfQuarterNow.ToString("ddMMyyyy");
                var redisQuarterHomeCache = RedisService<QuarterHomeCache>.GetInstance(_RedisConnection);
                QuarterHomeCache QuarterHomeCache;
                QuarterHomeCache = redisQuarterHomeCache.Get<QuarterHomeCache>(DateFrom_ToKeyQ);
                if (QuarterHomeCache != null)
                {
                    QuarterHomeCache.ListDataQuarter = modelResult.ListDataQuarter;
                    QuarterHomeCache.ListLateTopInQuarter = modelResult.ListLateTopInQuarter;
                    QuarterHomeCache.ListEarlyTopInQuarter = modelResult.ListEarlyTopInQuarter;
                    QuarterHomeCache.DataChartQuarter = modelResult.DataChartQuarter;

                    redisQuarterHomeCache.Replace(DateFrom_ToKeyQ, QuarterHomeCache);
                }
                else
                {

                    QuarterHomeCache = new QuarterHomeCache();
                    QuarterHomeCache.ListDataQuarter = modelResult.ListDataQuarter;
                    QuarterHomeCache.ListLateTopInQuarter = modelResult.ListLateTopInQuarter;
                    QuarterHomeCache.ListEarlyTopInQuarter = modelResult.ListEarlyTopInQuarter;
                    QuarterHomeCache.DataChartQuarter = modelResult.DataChartQuarter;

                    redisQuarterHomeCache.Add(DateFrom_ToKeyQ, QuarterHomeCache);

                }
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }
    }


}