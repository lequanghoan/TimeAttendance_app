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

namespace TimeAttendance.Business
{
    public class AttendanceLogBusiness
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();

        public AttendanceLogSearchResultObject GetListAttendanceLog(AttendanceLogSearchCondition model)
        {
            model.DateFrom = DateTime.ParseExact(model.DateFrom.ToString("dd/MM/yyyy") + " " + model.TimeFrom + ":00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
            model.DateTo = DateTime.ParseExact(model.DateTo.ToString("dd/MM/yyyy") + " " + model.TimeTo + ":59.999", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
            try
            {
                var listAttendanceLog = (from a in db.AttendanceLog.AsNoTracking()
                                         join e in db.Employee.AsNoTracking() on a.EmployeeId equals e.EmployeeId into g
                                         from ae in g.DefaultIfEmpty()
                                         join d in db.Department.AsNoTracking() on ae.DepartmentId equals d.DepartmentId into gr
                                         from aed in gr.DefaultIfEmpty()
                                         join f in db.JobTitle.AsNoTracking() on ae.JobTitleId equals f.JobTitleId into af
                                         from af1 in af.DefaultIfEmpty()
                                         where (string.IsNullOrEmpty(model.EmployeeCode) || ae.Code.Contains(model.EmployeeCode))
                                         && (string.IsNullOrEmpty(model.EmployeeName) || ae.Name.Contains(model.EmployeeName))
                                         && (string.IsNullOrEmpty(model.CameraIPAddress) || a.CameraIPAdress.Contains(model.CameraIPAddress))
                                         && (string.IsNullOrEmpty(model.DepartmentId) || ae.DepartmentId.Equals(model.DepartmentId))
                                         && (model.FaceCount == null || a.FaceCount == model.FaceCount)
                                         orderby a.Date descending
                                         select new AttendanceLogSearchResult()
                                         {
                                             AttendanceLogId = a.AttendanceLogId,
                                             EmployeeName = ae.Name,
                                             EmployeeId=a.EmployeeId,
                                             EmployeeCode = ae.Code,
                                             DepartmentName = aed.Name,
                                             JobTitleName = af1.Name,
                                             Date = (DateTime)a.Date,
                                             Confidence = a.Confidence,
                                             CameraIPAddress = a.CameraIPAdress,
                                             ImageLink = a.ImageLink,
                                             ObjSelect = false,
                                             FaceCount=a.FaceCount,
                                             ImageFace = a.ImageFace,
                                         }).AsQueryable();

                listAttendanceLog = listAttendanceLog.Where(r => r.Date <= model.DateTo);
                listAttendanceLog = listAttendanceLog.Where(r => r.Date >= model.DateFrom);
                listAttendanceLog = listAttendanceLog.Where(r => r.Confidence >= model.ConfidenceFrom);
                listAttendanceLog = listAttendanceLog.Where(r => r.Confidence <= model.ConfidenceTo);

                var count = listAttendanceLog.Select(u => u.AttendanceLogId).ToList().Count;
                var listResult = SQLHelpper.OrderBy(listAttendanceLog, model.OrderBy, model.OrderType).Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                AttendanceLogSearchResultObject result = new AttendanceLogSearchResultObject()
                {
                    ListResult = listResult,
                    TotalItem = count
                };
                return result;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public List<AttendanceLogSearchResult> GetListEmployee(string departmentId)
        {
            try
            {
                var listEmployee = (from a in db.Employee.AsNoTracking()
                                    where a.DepartmentId.Equals(departmentId)
                                    orderby a.Name
                                    select new AttendanceLogSearchResult()
                                    {
                                        EmployeeId = a.EmployeeId,
                                        EmployeeName = a.Name,
                                        EmployeeCode = a.Code,
                                        DepartmentId = a.DepartmentId,
                                        EmployeeCodeName = a.Code + " - " + a.Name
                                    }).ToList();
                return listEmployee;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public List<DepartmentSearchResult> GetListDepartment()
        {
            try
            {
                var listDepartment = (from a in db.Department.AsNoTracking()
                                      orderby a.Name
                                      select new DepartmentSearchResult()
                                      {
                                          DepartmentId = a.DepartmentId,
                                          Name = a.Name
                                      }).ToList();
                return listDepartment;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public int UpdateAttendanceLog(AttendanceLogSearchResult model)
        {
            var timeIn = db.SystemParam.Where(r => r.ParamName.Equals(Constants.ConfigTimeIn)).FirstOrDefault().ParamValue;
            var timeOut = db.SystemParam.Where(r => r.ParamName.Equals(Constants.ConfigTimeOut)).FirstOrDefault().ParamValue;

            using (var trans = db.Database.BeginTransaction())
            {
                List<DateTime> listDate = new List<DateTime>();
                List<DateTime> listDateOldEmployee = new List<DateTime>();

                try
                {
                    var attendanceLogNeedUpdate = db.AttendanceLog.FirstOrDefault(u => u.AttendanceLogId.Equals(model.AttendanceLogId));
                    AttendanceLogSearchResult attendanceLogNeedUpdateModel = new AttendanceLogSearchResult()
                    {
                        AttendanceLogId = attendanceLogNeedUpdate.AttendanceLogId,
                        CameraIPAddress = attendanceLogNeedUpdate.CameraIPAdress,
                        ClientIPAddress = attendanceLogNeedUpdate.ClientIPAddress,
                        Confidence = attendanceLogNeedUpdate.Confidence,
                        ImageLink = attendanceLogNeedUpdate.ImageLink,
                        EmployeeId = attendanceLogNeedUpdate.EmployeeId,
                        Date = (DateTime)attendanceLogNeedUpdate.Date
                    };

                    if (attendanceLogNeedUpdate != null)
                    {
                        //Update cho nhân viên mới
                        listDate = (from a in db.AttendanceLog.AsNoTracking()
                                    where a.EmployeeId.Equals(model.EmployeeId)
                                    && attendanceLogNeedUpdate.Date.Value.Day.Equals(a.Date.Value.Day)
                                    select a.Date.Value).ToList();

                        if (listDate.Count > 0)
                        {
                            listDate.Sort();
                            var listTimeAttendanceLog = db.TimeAttendanceLog.Where(r => r.EmployeeId.Equals(model.EmployeeId) && r.Date.Day.Equals(attendanceLogNeedUpdate.Date.Value.Day)).ToList();

                            //Thêm mới bảng TimeAttendanceLog khi nhân viên này chưa có bản ghi nào trong TimeAttendanceLog
                            if (listTimeAttendanceLog.Count() == 0)
                            {
                                AddTimeAttendanceLog(attendanceLogNeedUpdateModel, model.EmployeeId, timeIn, timeOut);
                                attendanceLogNeedUpdate.EmployeeId = model.EmployeeId;
                            }
                            else
                            {
                                //Update bảng TimeAttendanceLog khi đã có bản ghi trong đó
                                UpdateTimeAttendanceLog(listDate, attendanceLogNeedUpdateModel, model.EmployeeId, timeIn, timeOut);
                                attendanceLogNeedUpdate.EmployeeId = model.EmployeeId;
                            }
                        }
                        else
                        {
                            AddTimeAttendanceLog(attendanceLogNeedUpdateModel, model.EmployeeId, timeIn, timeOut);
                            attendanceLogNeedUpdate.EmployeeId = model.EmployeeId;
                        }

                        //Update cho nhân viên cũ sau khi chuyển đổi
                        listDateOldEmployee = (from a in db.AttendanceLog.AsNoTracking()
                                    where a.EmployeeId.Equals(model.EmployeeOldId)
                                    && attendanceLogNeedUpdate.Date.Value.Day.Equals(a.Date.Value.Day)
                                    select a.Date.Value).ToList();

                        var timeBegin = DateTime.Parse(((DateTime)attendanceLogNeedUpdateModel.Date).ToShortDateString() + " " + timeIn); //giờ quy định vào(8:30)
                        var timeEnd = DateTime.Parse(((DateTime)attendanceLogNeedUpdateModel.Date).ToShortDateString() + " " + timeOut); // Giờ quy định ra(18:00)

                        if (listDateOldEmployee.Count == 1)
                        {
                            //Xóa bản ghi của nhân viên cũ bên bảng TimeAttendanceLog
                            var oldEmployee = db.TimeAttendanceLog.FirstOrDefault(r => r.EmployeeId.Equals(model.EmployeeOldId) && r.Date.Day.Equals(attendanceLogNeedUpdateModel.Date.Day));
                            db.TimeAttendanceLog.Remove(oldEmployee);
                        }
                        else if (listDateOldEmployee.Count == 2)
                        {
                            //Xóa giá trị date trong list sẽ bị chuyển đổi cho nhân vien khác
                            listDateOldEmployee.Remove(attendanceLogNeedUpdateModel.Date);

                            var oldEmployee = db.TimeAttendanceLog.FirstOrDefault(r => r.EmployeeId.Equals(model.EmployeeOldId) && r.Date.Day.Equals(attendanceLogNeedUpdateModel.Date.Day));
                            oldEmployee.TimeIn = listDateOldEmployee[0];
                            oldEmployee.TimeOut = listDateOldEmployee[0];
                            oldEmployee.Total = 0;
                            if (timeBegin > oldEmployee.TimeIn)
                            {
                                oldEmployee.LateMinutes = 0;
                            }
                            else
                            {
                                oldEmployee.LateMinutes = TimeSpanCalculate(timeBegin, (DateTime)oldEmployee.TimeIn);
                            }

                            if (timeEnd < oldEmployee.TimeOut)
                            {
                                oldEmployee.EarlyMinutes = 0;
                            }
                            else
                            {
                                oldEmployee.EarlyMinutes = TimeSpanCalculate((DateTime)oldEmployee.TimeOut, timeEnd);
                            }
                        }
                        else
                        {
                            //Xóa giá trị date trong list sẽ bị chuyển đổi cho nhân vien khác
                            listDateOldEmployee.Remove(attendanceLogNeedUpdateModel.Date);

                            listDateOldEmployee.Sort();
                            var oldEmployee = db.TimeAttendanceLog.FirstOrDefault(r => r.EmployeeId.Equals(model.EmployeeOldId) && r.Date.Day.Equals(attendanceLogNeedUpdateModel.Date.Day));
                            oldEmployee.TimeIn = listDateOldEmployee[0];
                            oldEmployee.TimeOut = listDateOldEmployee[listDateOldEmployee.Count - 1];

                            oldEmployee.Total = TimeSpanCalculate((DateTime)oldEmployee.TimeIn, (DateTime)oldEmployee.TimeOut);

                            if (timeBegin > oldEmployee.TimeIn)
                            {
                                oldEmployee.LateMinutes = 0;
                            }
                            else
                            {
                                oldEmployee.LateMinutes = TimeSpanCalculate(timeBegin, (DateTime)oldEmployee.TimeIn);
                            }

                            if (timeEnd < oldEmployee.TimeOut)
                            {
                                oldEmployee.EarlyMinutes = 0;
                            }
                            else
                            {
                                oldEmployee.EarlyMinutes = TimeSpanCalculate((DateTime)oldEmployee.TimeOut, timeEnd);
                            }
                        }

                        trans.Commit();
                        db.SaveChanges();
                    }
                    else
                    {
                        return Constants.NOT_FOUND;
                    }

                    return Constants.OK;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }

        public int DeleteAttendanceLog(string attendanceLogId)
        {
            var timeIn = db.SystemParam.Where(r => r.ParamName.Equals(Constants.ConfigTimeIn)).FirstOrDefault().ParamValue;
            var timeOut = db.SystemParam.Where(r => r.ParamName.Equals(Constants.ConfigTimeOut)).FirstOrDefault().ParamValue;

            List<DateTime> listDate = new List<DateTime>();
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var attendanceLogNeedDelete = db.AttendanceLog.FirstOrDefault(u => u.AttendanceLogId.Equals(attendanceLogId));
                    if (attendanceLogNeedDelete != null)
                    {
                        listDate = (from a in db.AttendanceLog.AsNoTracking()
                                    where a.EmployeeId.Equals(attendanceLogNeedDelete.EmployeeId)
                                    && attendanceLogNeedDelete.Date.Value.Day.Equals(a.Date.Value.Day)
                                    select a.Date.Value).ToList();

                        listDate.Sort();
                        if (listDate[0].Equals(attendanceLogNeedDelete.Date))
                        {
                            var timeAttendanceLog = db.TimeAttendanceLog.FirstOrDefault(r => r.EmployeeId.Equals(attendanceLogNeedDelete.EmployeeId) && r.Date.Day.Equals(attendanceLogNeedDelete.Date.Value.Day));
                            if (listDate.Count >= 2)
                            {
                                timeAttendanceLog.TimeIn = listDate[1];
                                timeAttendanceLog.Total = TimeSpanCalculate((DateTime)timeAttendanceLog.TimeIn, (DateTime)timeAttendanceLog.TimeOut);

                                var timeBegin = DateTime.Parse(((DateTime)attendanceLogNeedDelete.Date).ToShortDateString() + " " + timeIn); //giờ quy định vào(8:30)

                                if (timeBegin > timeAttendanceLog.TimeIn)
                                {
                                    timeAttendanceLog.LateMinutes = 0;
                                }
                                else
                                {
                                    timeAttendanceLog.LateMinutes = TimeSpanCalculate(timeBegin, (DateTime)timeAttendanceLog.TimeIn);
                                }
                            }
                            else
                            {
                                db.TimeAttendanceLog.Remove(timeAttendanceLog);
                            }
                        }

                        if (listDate[listDate.Count - 1].Equals(attendanceLogNeedDelete.Date))
                        {
                            var timeAttendanceLog = db.TimeAttendanceLog.FirstOrDefault(r => r.EmployeeId.Equals(attendanceLogNeedDelete.EmployeeId) && r.Date.Day.Equals(attendanceLogNeedDelete.Date.Value.Day));
                            if (listDate.Count >= 2)
                            {
                                timeAttendanceLog.TimeOut = listDate[listDate.Count - 2];
                                timeAttendanceLog.Total = TimeSpanCalculate((DateTime)timeAttendanceLog.TimeIn, (DateTime)timeAttendanceLog.TimeOut);

                                var timeEnd = DateTime.Parse(((DateTime)attendanceLogNeedDelete.Date).ToShortDateString() + " " + timeOut); // Giờ quy định ra(18:00)
                                if (timeEnd < timeAttendanceLog.TimeOut)
                                {
                                    timeAttendanceLog.EarlyMinutes = 0;
                                }
                                else
                                {
                                    timeAttendanceLog.EarlyMinutes = TimeSpanCalculate((DateTime)timeAttendanceLog.TimeOut, timeEnd);
                                }
                            }
                            else
                            {
                                db.TimeAttendanceLog.Remove(timeAttendanceLog);
                            }
                        }

                        db.AttendanceLog.Remove(attendanceLogNeedDelete);
                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        return Constants.NOT_FOUND;
                    }

                    return Constants.OK;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }

        private void AddTimeAttendanceLog(AttendanceLogSearchResult attendanceLogNeedUpdateModel, string employeeId, string TimeIn, string TimeOut)
        {
            TimeAttendanceLog add = new TimeAttendanceLog();
            add.TimeAttendanceLogId = Guid.NewGuid().ToString();
            add.Date = DateTime.Parse(attendanceLogNeedUpdateModel.Date.ToShortDateString());
            add.EmployeeId = employeeId;
            add.ImageIn = attendanceLogNeedUpdateModel.ImageLink;
            add.TimeIn = attendanceLogNeedUpdateModel.Date;
            add.TimeOut = attendanceLogNeedUpdateModel.Date;
            add.Total = 0;
            add.ImageOut = attendanceLogNeedUpdateModel.ImageLink;

            var timeBegin = DateTime.Parse(attendanceLogNeedUpdateModel.Date.ToShortDateString() + " " + TimeIn); //giờ quy định vào(8:30)

            if (timeBegin > attendanceLogNeedUpdateModel.Date)
            {
                add.LateMinutes = 0;
            }
            else
            {
                add.LateMinutes = TimeSpanCalculate(timeBegin, attendanceLogNeedUpdateModel.Date);
            }

            var timeEnd = DateTime.Parse(attendanceLogNeedUpdateModel.Date.ToShortDateString() + " " + TimeOut); // Giờ quy định ra(18:00)

            if (timeEnd < attendanceLogNeedUpdateModel.Date)
            {
                add.EarlyMinutes = 0;
            }
            else
            {
                add.EarlyMinutes = TimeSpanCalculate(attendanceLogNeedUpdateModel.Date, timeEnd);
            }
            db.TimeAttendanceLog.Add(add);
        }

        private void UpdateTimeAttendanceLog(List<DateTime> listDate, AttendanceLogSearchResult attendanceLogNeedUpdateModel, string employeeId, string timeIn, string timeOut)
        {
            //listDate: list date trong bảng AttendanceLog của đối tượng sẽ chuyển đến
            if (listDate[0] > attendanceLogNeedUpdateModel.Date) //Nếu date cần cập nhật nhỏ hơn < min date của đối tượng chuyển đến thì timeIn = date này 
            {
                var timeAttendanceLog = db.TimeAttendanceLog.FirstOrDefault(u => u.Date.Day.Equals(attendanceLogNeedUpdateModel.Date.Day) && u.EmployeeId.Equals(employeeId));
                //cập nhật thời gian vào trong timeAttendanceLog = thời gian của attendanceLog
                timeAttendanceLog.TimeIn = attendanceLogNeedUpdateModel.Date;
                timeAttendanceLog.Total = TimeSpanCalculate((DateTime)timeAttendanceLog.TimeIn, (DateTime)timeAttendanceLog.TimeOut);

                //timeBegin: thời gian băt đầu vào gồm cả ngày tháng, timeIn: thời gian bắt đầu vào chỉ có giờ (8:30)
                var timeBegin = DateTime.Parse(attendanceLogNeedUpdateModel.Date.ToShortDateString() + " " + timeIn);

                if (timeBegin > attendanceLogNeedUpdateModel.Date)
                {
                    timeAttendanceLog.LateMinutes = 0;
                }
                else
                {
                    timeAttendanceLog.LateMinutes = TimeSpanCalculate(timeBegin, attendanceLogNeedUpdateModel.Date);
                }
            }

            if (listDate[listDate.Count - 1] < attendanceLogNeedUpdateModel.Date)
            {
                var timeAttendanceLog = db.TimeAttendanceLog.FirstOrDefault(u => u.Date.Day.Equals(attendanceLogNeedUpdateModel.Date.Day) && u.EmployeeId.Equals(employeeId));

                timeAttendanceLog.TimeOut = attendanceLogNeedUpdateModel.Date;
                timeAttendanceLog.Total = TimeSpanCalculate((DateTime)timeAttendanceLog.TimeIn, (DateTime)timeAttendanceLog.TimeOut);

                //timeOut: Giờ quy định ra(18:00)
                var timeEnd = DateTime.Parse(attendanceLogNeedUpdateModel.Date.ToShortDateString() + " " + timeOut);
                if (timeEnd < attendanceLogNeedUpdateModel.Date)
                {
                    timeAttendanceLog.EarlyMinutes = 0;
                }
                else
                {
                    timeAttendanceLog.EarlyMinutes = TimeSpanCalculate(attendanceLogNeedUpdateModel.Date, timeEnd);
                }
            }
        }

        private int TimeSpanCalculate(DateTime timeBegin, DateTime timeEnd)
        {
            TimeSpan timeSp = timeEnd - timeBegin;
            return timeSp.Hours * 60 + timeSp.Minutes;
        }

        public AttendanceLogUpdate GetAttendanceLogInfo(AttendanceLogSearchResult model)
        {
            try
            {
                AttendanceLogSearchResult data = (from a in db.AttendanceLog.AsNoTracking()
                                                  join e in db.Employee.AsNoTracking() on a.EmployeeId equals e.EmployeeId
                                                  where a.AttendanceLogId.Equals(model.AttendanceLogId)
                                                  select new AttendanceLogSearchResult()
                                                  {
                                                      AttendanceLogId = a.AttendanceLogId,
                                                      EmployeeId = a.EmployeeId,
                                                      EmployeeCode = e.Code,
                                                      EmployeeName = e.Name,
                                                      DepartmentId = e.DepartmentId
                                                  }).FirstOrDefault();
                var listEmployee = GetListEmployee(data.DepartmentId);
                AttendanceLogUpdate result = new AttendanceLogUpdate()
                {
                    ListEmployee = listEmployee,
                    AtendanceLogSearchResult = data
                };
                return result;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public AttendanceLogSearchResultObject ExportExcel(AttendanceLogSearchCondition model)
        {
            //Khởi tạo Excel
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            string pathClient = HttpContext.Current.Server.MapPath("/Template/ThongkeLuotVaoRa.xlsx");
            IWorkbook workbook = application.Workbooks.Open(pathClient);
            IWorksheet sheet = workbook.Worksheets[0];

            //Khởi tạo dữ liệu Model
            try
            {
                int index = 1;
                AttendanceLogSearchResultObject result = GetListAttendanceLog(model);
                var list = result.ListResult;
                foreach (var e in list)
                {
                    if (string.IsNullOrEmpty(e.EmployeeCode))
                    {
                        e.EmployeeCode = "Unknow";
                    }
                    if (string.IsNullOrEmpty(e.EmployeeName))
                    {
                        e.EmployeeName = "Unknow";
                    }
                    if (string.IsNullOrEmpty(e.DepartmentName))
                    {
                        e.DepartmentName = "Unknow";
                    }
                    e.DateString = e.Date.ToString("dd/MM/yyyy   HH:mm:ss");
                }
                var listExport = (from p in list
                                  select new
                                  {
                                      Index = index++,
                                      p.EmployeeCode,
                                      p.EmployeeName,
                                      p.DepartmentName,
                                      p.DateString,
                                      p.Confidence,
                                      p.CameraIPAddress,
                                  }).ToList();
                if (listExport.Count > 0)
                {
                    sheet.Range[6, 2, 6 + listExport.Count, 8].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    sheet.Range[6, 2, 6 + listExport.Count, 8].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    sheet.Range[6, 2, 6 + listExport.Count, 8].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    sheet.Range[6, 2, 6 + listExport.Count, 8].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;

                    sheet.ImportData(listExport, 7, 2, false);
                }

                //Xuất Excel
                string pathExport = "/Template/Export/" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "ThongkeVaoRa.xlsx";
                workbook.SaveAs(HttpContext.Current.Server.MapPath(pathExport));

                workbook.Close();
                excelEngine.Dispose();
                result.PathExport = pathExport;
                return result;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public string GetAvataEmp(string AttendanceLogId)
        {
            string rs = "";
            try
            {
                var data = (from a in db.Employee.AsNoTracking()
                            join b in db.AttendanceLog.AsNoTracking() 
                             on a.EmployeeId equals b.EmployeeId
                            where b.AttendanceLogId.Equals(AttendanceLogId)
                            select a.LinkImage).FirstOrDefault();
                if (!string.IsNullOrEmpty(data))
                {
                    rs= data.Split(';')[0];
                }
            }
            catch (Exception ex)
            {
            }
            return rs;
        }
    }
}
