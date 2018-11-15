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
using Syncfusion.XlsIO;
using System.Web;
using System.Globalization;
using System.IO;
using System.Web.Hosting;

namespace TimeAttendance.Business
{
    public class ComboboxBusiness
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();
        public List<ComboboxResult> GetAllDepartment()
        {
            List<ComboboxResult> searchResult = new List<ComboboxResult>();
            try
            {
                searchResult = (from a in db.Department.AsNoTracking()
                                orderby a.Name
                                select new ComboboxResult()
                                {
                                    Id = a.DepartmentId,
                                    Name = a.Name,
                                }).ToList();

                return searchResult;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public List<ComboboxResult> GetAllJobTitle()
        {
            List<ComboboxResult> searchResult = new List<ComboboxResult>();
            try
            {
                searchResult = (from a in db.JobTitle.AsNoTracking()
                                orderby a.Name
                                select new ComboboxResult()
                                {
                                    Id = a.JobTitleId,
                                    Name = a.Name,
                                }).ToList();

                return searchResult;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public List<InfoEmployeeModel> GetAllEmployee()
        {
            db = new TimeAttendanceEntities();
            try
            {
                var listResult = (from a in db.Employee.AsNoTracking()
                                  join b in db.Department.AsNoTracking() on a.DepartmentId equals b.DepartmentId
                                  join c in db.JobTitle.AsNoTracking() on a.JobTitleId equals c.JobTitleId
                                  select new InfoEmployeeModel()
                                  {
                                      EmployeeId = a.EmployeeId,
                                      DepartmentName = b.Name,
                                      JobTitleName = c.Name,
                                      FaceId = a.FaceId,
                                      Name = a.Name,
                                      Code = a.Code,
                                      DateOfBirth = a.DateOfBirth,
                                      GenderName = a.Gender == Constants.Male ? "Nam" : "Nữ",
                                      InComeDate = a.InComeDate,
                                      IdentifyCardNumber = a.IdentifyCardNumber,
                                      LinkImage = a.LinkImage,
                                  }).ToList();

                return listResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ConfigResult GetConfigResult()
        {
            db = new TimeAttendanceEntities();
            try
            {
                ConfigResult con = new ConfigResult();
                var system = db.SystemParam.Where(u => u.ParamName.Equals(Constants.ConfigTimeIn)
                || u.ParamName.Equals(Constants.ConfigTimeOut)
                || u.ParamName.Equals(Constants.ConfigPercent)
                 || u.ParamName.Equals(Constants.TimeAttendanceLog)
                ).ToList();
                var ConfigTimeIn = system.FirstOrDefault(u => u.ParamName.Equals(Constants.ConfigTimeIn));
                var ConfigTimeOut = system.FirstOrDefault(u => u.ParamName.Equals(Constants.ConfigTimeOut));
                var ConfigPercent = system.FirstOrDefault(u => u.ParamName.Equals(Constants.ConfigPercent));
                var TimeAttendanceLog = system.FirstOrDefault(u => u.ParamName.Equals(Constants.TimeAttendanceLog));
                if (ConfigTimeIn != null)
                {
                    con.TimeIn = ConfigTimeIn.ParamValue;
                }
                if (ConfigTimeOut != null)
                {
                    con.TimeOut = ConfigTimeOut.ParamValue;
                }
                if (ConfigPercent != null)
                {
                    con.Percent = int.Parse(ConfigPercent.ParamValue);
                }
                if (TimeAttendanceLog != null)
                {
                    con.TimeAttendanceLog = int.Parse(TimeAttendanceLog.ParamValue);
                }
                return con;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void UpdateConfigResult(ConfigResult con)
        {
            db = new TimeAttendanceEntities();
            try
            {
                var ConfigTimeIn = db.SystemParam.FirstOrDefault(u => u.ParamName.Equals(Constants.ConfigTimeIn));
                var ConfigTimeOut = db.SystemParam.FirstOrDefault(u => u.ParamName.Equals(Constants.ConfigTimeOut));
                var ConfigPercent = db.SystemParam.FirstOrDefault(u => u.ParamName.Equals(Constants.ConfigPercent));
                var TimeAttendanceLog = db.SystemParam.FirstOrDefault(u => u.ParamName.Equals(Constants.TimeAttendanceLog));
                if (ConfigTimeIn != null)
                {
                    ConfigTimeIn.ParamValue = con.TimeIn;
                }
                else
                {
                    ConfigTimeIn = new SystemParam();
                    ConfigTimeIn.SystemParamId = Guid.NewGuid().ToString(); ;
                    ConfigTimeIn.ParamValue = con.TimeIn;
                    ConfigTimeIn.ParamName = Constants.ConfigTimeIn;
                    db.SystemParam.Add(ConfigTimeIn);
                }
                if (ConfigTimeOut != null)
                {
                    ConfigTimeOut.ParamValue = con.TimeOut;
                }
                else
                {
                    ConfigTimeOut = new SystemParam();
                    ConfigTimeOut.SystemParamId = Guid.NewGuid().ToString(); ;
                    ConfigTimeOut.ParamValue = con.TimeOut;
                    ConfigTimeOut.ParamName = Constants.ConfigTimeOut;
                    db.SystemParam.Add(ConfigTimeOut);
                }
                if (ConfigPercent != null)
                {
                    ConfigPercent.ParamValue = con.Percent.ToString();
                }
                else
                {
                    ConfigPercent = new SystemParam();
                    ConfigPercent.SystemParamId = Guid.NewGuid().ToString(); ;
                    ConfigPercent.ParamValue = con.Percent.ToString();
                    ConfigPercent.ParamName = Constants.ConfigPercent;
                    db.SystemParam.Add(ConfigPercent);
                }
                if (TimeAttendanceLog != null)
                {
                    TimeAttendanceLog.ParamValue = con.TimeAttendanceLog.ToString();
                }
                else
                {
                    TimeAttendanceLog = new SystemParam();
                    TimeAttendanceLog.SystemParamId = Guid.NewGuid().ToString(); ;
                    TimeAttendanceLog.ParamValue = con.TimeAttendanceLog.ToString();
                    TimeAttendanceLog.ParamName = Constants.TimeAttendanceLog;
                    db.SystemParam.Add(TimeAttendanceLog);
                }
                db.SaveChanges();
                TimeAttendanceStatic.ConfidenceFix = con.Percent;
                TimeAttendanceStatic.TimeIn = con.TimeIn;
                TimeAttendanceStatic.TimeOut = con.TimeOut;
            }
            catch (Exception ex)
            {
            }
        }

        public double GetConfidenceFix()
        {
            try
            {
                var confidence = db.SystemParam.Where(r => r.ParamName.Equals(Constants.ConfigPercent)).FirstOrDefault();
                if (confidence != null && confidence.ParamValue != null && !string.IsNullOrEmpty(confidence.ParamValue))
                {
                    return double.Parse(confidence.ParamValue);
                }
            }
            catch (Exception ex)
            {

            }
            return 50;
        }

        /// <summary>
        /// Import khách hàng từ file mẫu
        /// </summary>
        public void ImportEmployee()
        {
            TimeAttendanceEntities db = new TimeAttendanceEntities();
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            IWorkbook workbook = application.Workbooks.Open(HttpContext.Current.Server.MapPath("/Template/MSB.xls"));

            IWorksheet sheet = workbook.Worksheets[0];

            //try
            //{
            sheet = workbook.Worksheets[0];
            var listDep = db.Department.Select(r => new { r.DepartmentId, r.Name }).ToList();
            if (sheet.Columns.Count() >= 5)
            {
                List<EmployeeModel> listEmployee = new List<EmployeeModel>();
                List<string> listDepartmentName = new List<string>();
                EmployeeModel modelEmployee;
                int count = 0;
                int total = 0;
                string pathFolder = "fileUpload/ImageEmployee/";
                string pathFolderServer = HostingEnvironment.MapPath("~/" + pathFolder);
                for (int indexRow = 4; indexRow <= sheet.Rows.Count(); indexRow++)
                {
                    total++;
                    modelEmployee = new EmployeeModel();
                    modelEmployee.Code = sheet.Range[indexRow, 3].Value.ToString().Trim();
                    modelEmployee.Name = sheet.Range[indexRow, 4].Value.ToString().Trim();
                    listDepartmentName.Add(sheet.Range[indexRow, 5].Value.ToString().Trim());
                    var itemDep = listDep.Where(r => r.Name.ToLower().Contains(sheet.Range[indexRow, 5].Value.ToString().Trim().ToLower())).FirstOrDefault();
                    if (itemDep != null)
                        modelEmployee.DepartmentId = itemDep.DepartmentId;
                    if (sheet.Range[indexRow, 6].HasDateTime)
                        modelEmployee.InComeDate = sheet.Range[indexRow, 6].DateTime;
                    else
                        modelEmployee.InComeDate = DateTime.ParseExact(sheet.Range[indexRow, 6].Text, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    listEmployee.Add(modelEmployee);
                }

                //listDepartmentName = listDepartmentName.GroupBy(r => r).Select(r=>r.Key).ToList();
                //Department department;
                //foreach (var item in listDepartmentName)
                //{
                //    department = new Department()
                //    {
                //        DepartmentId = Guid.NewGuid().ToString(),
                //        Name = item
                //    };
                //    db.Department.Add(department);
                //}
                //db.SaveChanges();

                IEnumerable<string> listFile = Directory.EnumerateFiles(HostingEnvironment.MapPath("~/fileUpload/ImageUserTemp/"), "*.*", SearchOption.AllDirectories).Where(r => r.ToLower().Contains(".jpg") || r.ToLower().Contains(".png")).ToList();
                EmployeeBusiness employeeBusiness = new EmployeeBusiness();
                Employee employee;
                int countOk = 0;
                int countError = 0;
                foreach (var item in listEmployee)
                {
                    try
                    {
                        string file = listFile.Where(r => r.Contains(item.Code.Replace("MSB", ""))).FirstOrDefault();
                        if (file != null)
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            fileInfo.CopyTo(HostingEnvironment.MapPath("~/fileUpload/ImageEmployee/" + fileInfo.Name), true);
                            item.LinkImage = "fileUpload/ImageEmployee/" + fileInfo.Name;
                            item.CreateBy = "US000001";
                            item.JobTitleId = "2a6ec39b-368c-488b-a019-9f171248f37a";
                            item.Gender = "2";
                            employeeBusiness.CreateEmployee(item);
                        }
                        countOk++;
                    }
                    catch (Exception ex)
                    {
                        countError++;
                    }
                    Task.Delay(50000);
                }

            }
        }
    }
}
