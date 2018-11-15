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
    public class NoAttendanceLogBusiness
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();

        public AttendanceLogSearchResultObject GetListNoAttendanceLog(NoAttendanceLogSearchCondition model)
        {
            try
            {
                List<string> listEmployeeId = new List<string>();

                listEmployeeId = (from t in db.TimeAttendanceLog.AsNoTracking()
                                             where model.DateFrom <= t.Date && t.Date <= model.DateTo
                                             select t.EmployeeId).ToList();

                var listEmployee = (from e in db.Employee.AsNoTracking()
                                    join d in db.Department.AsNoTracking() on e.DepartmentId equals d.DepartmentId
                                    join j in db.JobTitle.AsNoTracking() on e.JobTitleId equals j.JobTitleId
                                    where (string.IsNullOrEmpty(model.DepartmentId) || e.DepartmentId.Equals(model.DepartmentId))
                                    && (string.IsNullOrEmpty(model.JobTitleId) || e.JobTitleId.Equals(model.JobTitleId))
                                    && (string.IsNullOrEmpty(model.EmployeeCode) || e.Code.Contains(model.EmployeeCode))
                                    && (string.IsNullOrEmpty(model.EmployeeName) || e.Name.Contains(model.EmployeeName))
                                    && !listEmployeeId.Contains(e.EmployeeId)
                                    select new AttendanceLogSearchResult()
                                    {
                                        EmployeeId = e.EmployeeId,
                                        EmployeeName = e.Name,
                                        EmployeeCode = e.Code,
                                        DepartmentName = d.Name,
                                        JobTitleName = j.Name,
                                    }).AsQueryable();

                var count = listEmployee.ToList().Count;
                var listResult = SQLHelpper.OrderBy(listEmployee, model.OrderBy, model.OrderType).Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
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

        public AttendanceLogSearchResultObject ExportExcel(NoAttendanceLogSearchCondition model)
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
                AttendanceLogSearchResultObject result = GetListNoAttendanceLog(model);
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
    }
}
