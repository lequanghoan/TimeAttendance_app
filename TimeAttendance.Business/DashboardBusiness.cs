using TimeAttendance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Model.SearchCondition;
using TimeAttendance.Model.SearchResults;
using TimeAttendance.Utils;
using System.Configuration;
using TimeAttendance.Model.Repositories;
using Microsoft.ProjectOxford.Face.Contract;
using System.Collections.ObjectModel;
using FaceVipService;
using System.IO;
using Syncfusion.XlsIO;
using System.Web;
using System.Globalization;
using TimeAttendance.Caching;
using TimeAttendance.Model.CacheModel;

namespace TimeAttendance.Business
{
    public class DashboardBusiness
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();

        public DashboardResultModel GetDataDashboard()
        {
            var RedisConnection = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"];
            DashboardResultModel modelResult = new DashboardResultModel();
            try
            {
                //var lstEmpId = db.Employee.Select(r => r.EmployeeId);
                //modelResult.TotalEmployee = lstEmpId.Count();

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

                //lấy dữ liệu trong ngày
                //dateFrom = DateTimeUtils.ConvertDateFrom(modelResult.DateNow);
                //dateTo = DateTimeUtils.ConvertDateTo(modelResult.DateNow);
                //var listDayNow = db.TimeAttendanceLog.AsNoTracking().Where(u => u.Date >= dateFrom && u.Date <= dateTo).AsQueryable();


                //#region  //Thống kê chấm công ngày hiện tại và ngày trước
                //#region //tinh toán
                //modelResult.ListDataDay = new List<DataResultModel>();
                //modelResult.ListDataDay.Add(new DataResultModel()
                //{
                //    Name = "Ngày " + modelResult.DateNow.ToString("dd/MM/yyyy"),
                //    TotalLate = listDayNow.Where(u => u.LateMinutes.HasValue && u.LateMinutes > 0).Count(),
                //    TotalEarly = listDayNow.Where(u => u.EarlyMinutes.HasValue && u.EarlyMinutes > 0).Count(),
                //    TotalAbsent = listDayNow.Where(u => !lstEmpId.Contains(u.EmployeeId)).Count()
                //});

                //var dataDayNow = modelResult.ListDataDay.First();
                //modelResult.LatePercent = Math.Round((double)(dataDayNow.TotalLate / modelResult.TotalEmployee) * 100, 2);
                //modelResult.EarlyPercent = Math.Round((double)(dataDayNow.TotalEarly / modelResult.TotalEmployee) * 100, 2);
                //modelResult.AbsentPercent = Math.Round((double)(dataDayNow.TotalAbsent / modelResult.TotalEmployee) * 100, 2);
                //#endregion

                //#region[lấy cache ngày]

                //var DateNowKey = "AT:DayHome:" + modelResult.DateNow.ToString("ddMMyyyy");
                //var redisDayHomeCache = RedisService<DayHomeCache>.GetInstance(RedisConnection);
                //DayHomeCache DayHomeCache;
                //DayHomeCache = redisDayHomeCache.Get<DayHomeCache>(DateNowKey);
                //if (DayHomeCache != null)
                //{
                //    DayHomeCache.ListDataDay = modelResult.ListDataDay;
                //    DayHomeCache.LatePercent = modelResult.LatePercent;
                //    DayHomeCache.EarlyPercent = modelResult.EarlyPercent;
                //    DayHomeCache.AbsentPercent = modelResult.AbsentPercent;
                //    //add vào cache
                //    redisDayHomeCache.Replace(DateNowKey, DayHomeCache);

                //    //lay cache hôm trước
                //    var DateBeforKey = "AT:DayHome:" + modelResult.DateNow.AddDays(-1).ToString("ddMMyyyy");
                //    var DayHomeCacheBefor = redisDayHomeCache.Get<DayHomeCache>(DateBeforKey);
                //    if (DayHomeCacheBefor != null)
                //    {
                //        modelResult.ListDataDay.Add(DayHomeCacheBefor.ListDataDay[0]);
                //    }
                //}
                //else
                //{
                //    DayHomeCache = new DayHomeCache();

                //    DayHomeCache.ListDataDay = modelResult.ListDataDay;
                //    DayHomeCache.LatePercent = modelResult.LatePercent;
                //    DayHomeCache.EarlyPercent = modelResult.EarlyPercent;
                //    DayHomeCache.AbsentPercent = modelResult.AbsentPercent;
                //    //add vào cache
                //    redisDayHomeCache.Add(DateNowKey, DayHomeCache);
                //}
                //#endregion

                //#endregion

                #region  //Thống kê chấm công tuần hiện tại và tuần trước
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfWeekNow);
                dateTo = DateTimeUtils.ConvertDateTo(lastDayOfWeekNow);
                #region[lấy cache ngày]
                var DateFrom_ToKeyW = "AT:WeekHome:" + firstDayOfWeekNow.ToString("ddMMyyyy") + "_" + lastDayOfWeekNow.ToString("ddMMyyyy");
                var redisWeekHomeCache = RedisService<WeekHomeCache>.GetInstance(RedisConnection);
                WeekHomeCache WeekHomeCache;
                WeekHomeCache = redisWeekHomeCache.Get<WeekHomeCache>(DateFrom_ToKeyW);

                modelResult.ListDataWeek = new List<DataResultModel>();
                modelResult.ListLateTopInWeek = new List<DataTopModel>();
                modelResult.ListEarlyTopInWeek = new List<DataTopModel>();
                modelResult.DataChartWeek = new DataChartModel();

                if (WeekHomeCache != null)
                {
                    modelResult.ListDataWeek = WeekHomeCache.ListDataWeek;
                    modelResult.ListLateTopInWeek = WeekHomeCache.ListLateTopInWeek;
                    modelResult.ListEarlyTopInWeek = WeekHomeCache.ListEarlyTopInWeek;
                    modelResult.DataChartWeek = WeekHomeCache.DataChartWeek;

                    //lấy cache tuần trước
                    var DateFrom_ToKeyWBefor = "AT:WeekHome:" + firstDayOfWeekBefore.ToString("ddMMyyyy") + "_" + lastDayOfWeekBefore.ToString("ddMMyyyy");
                    var WeekHomeCacheBefor = redisWeekHomeCache.Get<WeekHomeCache>(DateFrom_ToKeyWBefor);
                    if (WeekHomeCacheBefor != null)
                    {
                        modelResult.ListDataWeek.Add(WeekHomeCacheBefor.ListDataWeek[0]);
                    }
                }

                #endregion
                #endregion

                #region  //Thống kê chấm công tháng hiện tại và tháng trước
                dateFrom = DateTimeUtils.ConvertDateFrom(firstDayOfMonthNow);
                dateTo = DateTimeUtils.ConvertDateTo(lastDayOfMonthNow);
                #region[lấy cache ngày]
                var DateFrom_ToKeyM = "AT:MonthHome:" + firstDayOfMonthNow.ToString("ddMMyyyy") + "_" + lastDayOfMonthNow.ToString("ddMMyyyy");
                var redisMonthHomeCache = RedisService<MonthHomeCache>.GetInstance(RedisConnection);
                MonthHomeCache MonthHomeCache;
                MonthHomeCache = redisMonthHomeCache.Get<MonthHomeCache>(DateFrom_ToKeyM);
                if (MonthHomeCache != null)
                {
                    modelResult.ListDataMonth = MonthHomeCache.ListDataMonth; ;
                    modelResult.ListLateTopInMonth = MonthHomeCache.ListLateTopInMonth;
                    modelResult.ListEarlyTopInMonth = MonthHomeCache.ListEarlyTopInMonth;
                    modelResult.DataChartMonth = MonthHomeCache.DataChartMonth;
                    //lấy cache cũ
                    var DateFrom_ToKeyMBefor = "AT:MonthHome:" + firstDayOfMonthBefore.ToString("ddMMyyyy") + "_" + lastDayOfMonthBefore.ToString("ddMMyyyy");
                    var MonthHomeCacheBefor = redisMonthHomeCache.Get<MonthHomeCache>(DateFrom_ToKeyMBefor);
                    if (MonthHomeCacheBefor != null)
                    {
                        modelResult.ListDataMonth.Add(MonthHomeCacheBefor.ListDataMonth[0]);
                    }
                }

                #endregion


                #endregion

                #region  //Thống kê chấm công quý hiện tại và quý trước
                #region[lấy cache ngày]
                var DateFrom_ToKeyQ = "AT:QuarterHome:" + firstDayOfQuarterNow.ToString("ddMMyyyy") + "_" + lastDayOfQuarterNow.ToString("ddMMyyyy");
                var redisQuarterHomeCache = RedisService<QuarterHomeCache>.GetInstance(RedisConnection);
                QuarterHomeCache QuarterHomeCache;
                QuarterHomeCache = redisQuarterHomeCache.Get<QuarterHomeCache>(DateFrom_ToKeyQ);
                if (QuarterHomeCache != null)
                {
                    modelResult.ListDataQuarter = QuarterHomeCache.ListDataQuarter; ;
                    modelResult.ListLateTopInQuarter = QuarterHomeCache.ListLateTopInQuarter;
                    modelResult.ListEarlyTopInQuarter = QuarterHomeCache.ListEarlyTopInQuarter;
                    modelResult.DataChartQuarter = QuarterHomeCache.DataChartQuarter;
                    //lây cache quý trước
                    var DateFrom_ToKeyQBefor = "AT:QuarterHome:" + firstDayOfQuarterBefore.ToString("ddMMyyyy") + "_" + lastDayOfQuarterBefore.ToString("ddMMyyyy");
                    var QuarterHomeCacheBefor = redisQuarterHomeCache.Get<QuarterHomeCache>(DateFrom_ToKeyQBefor);
                    if (QuarterHomeCacheBefor != null)
                    {
                        modelResult.ListDataQuarter.Add(QuarterHomeCacheBefor.ListDataQuarter[0]);
                    }
                }

                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
            return modelResult;
        }
    }
}
