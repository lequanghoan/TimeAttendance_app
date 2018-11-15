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
using System.Web.Hosting;
using TimeAttendance.Caching;
using TimeAttendance.Model.CacheModel;

namespace TimeAttendance.Business
{
    public class EmployeeBusiness
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();
        private AuthContext authContext = new AuthContext();
        public ObservableCollection<PersistedFace> SelectedPersonFaces { get; set; } = new ObservableCollection<PersistedFace>();
        public ObservableCollection<Person> PersonsInCurrentGroup { get; set; } = new ObservableCollection<Person>();
        string NhomNV = ConfigurationManager.AppSettings["GroupTimeAttendance"];
        public SearchResultObject<EmployeeSearchResult> SearchEmployee(EmployeeSearchCondition searchCondition)
        {
            db = new TimeAttendanceEntities();
            int index = 1;
            SearchResultObject<EmployeeSearchResult> searchResult = new SearchResultObject<EmployeeSearchResult>();
            try
            {

                var listmodel = (from a in db.Employee.AsNoTracking()
                                 join b in db.Department.AsNoTracking() on a.DepartmentId equals b.DepartmentId
                                 join c in db.JobTitle.AsNoTracking() on a.JobTitleId equals c.JobTitleId
                                 where (string.IsNullOrEmpty(searchCondition.Code) || a.Code.ToLower().Contains(searchCondition.Code.ToLower()))
                                 && (string.IsNullOrEmpty(searchCondition.Name) || a.Name.ToLower().Contains(searchCondition.Name.ToLower()))
                                 && (string.IsNullOrEmpty(searchCondition.IdentifyCardNumber) || a.IdentifyCardNumber.ToLower().Contains(searchCondition.IdentifyCardNumber.ToLower()))
                                 && (string.IsNullOrEmpty(searchCondition.Address) || a.Address.ToLower().Contains(searchCondition.Address.ToLower()))
                                 && (searchCondition.Gender == -1 || a.Gender == searchCondition.Gender)
                                 && (string.IsNullOrEmpty(searchCondition.JobTitleId) || a.JobTitleId.Equals(searchCondition.JobTitleId))
                                 && (string.IsNullOrEmpty(searchCondition.DepartmentId) || a.DepartmentId.Equals(searchCondition.DepartmentId))
                                 select new EmployeeSearchResult()
                                 {
                                     EmployeeId = a.EmployeeId,
                                     DepartmentName = b.Name,
                                     JobTitleName = c.Name,
                                     FaceId = a.FaceId,
                                     Name = a.Name,
                                     Code = a.Code,
                                     DateOfBirth = a.DateOfBirth,
                                     Gender = a.Gender == Constants.Male ? "Nam" : "Nữ",
                                     InComeDate = a.InComeDate,
                                     OutComeDate = a.OutComeDate,
                                     Address = a.Address,
                                     Description = a.Description,
                                     IdentifyCardNumber = a.IdentifyCardNumber,
                                     LinkImage = a.LinkImage,
                                     EmpSelect = false,
                                 }).AsQueryable();
                if (searchCondition.DateFrom.HasValue)
                {
                    searchCondition.DateFrom = DateTimeUtils.ConvertDateFrom(searchCondition.DateFrom);
                    listmodel = listmodel.Where(r => r.InComeDate >= searchCondition.DateFrom);
                }
                if (searchCondition.DateTo.HasValue)
                {
                    searchCondition.DateTo = DateTimeUtils.ConvertDateTo(searchCondition.DateTo);
                    listmodel = listmodel.Where(r => r.InComeDate <= searchCondition.DateTo);
                }

                searchResult.TotalItem = listmodel.Select(r => r.EmployeeId).Count();
                searchResult.ListResult = SQLHelpper.OrderBy(listmodel, searchCondition.OrderBy, searchCondition.OrderType).Skip((searchCondition.PageNumber - 1) * searchCondition.PageSize)
                            .Take(searchCondition.PageSize).ToList();
                foreach (var item in searchResult.ListResult)
                {
                    item.Avata = string.IsNullOrEmpty(item.LinkImage) ? "" : item.LinkImage.Split(';')[0];
                }
                return searchResult;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }
        public List<EmployeeSearchResult> SearchEmployeeApp(EmployeeSearchCondition searchCondition)
        {
            string img;
            db = new TimeAttendanceEntities();
            int index = 1;
            List<EmployeeSearchResult> searchResult = new List<EmployeeSearchResult>();
            try
            {
                var listmodel = (from a in db.Employee.AsNoTracking()
                                 join b in db.Department.AsNoTracking() on a.DepartmentId equals b.DepartmentId
                                 join c in db.JobTitle.AsNoTracking() on a.JobTitleId equals c.JobTitleId
                                 where (string.IsNullOrEmpty(searchCondition.Code) || a.Code.ToLower().Contains(searchCondition.Code.ToLower()))
                                 && (string.IsNullOrEmpty(searchCondition.Name) || a.Name.ToLower().Contains(searchCondition.Name.ToLower()))
                                 && (string.IsNullOrEmpty(searchCondition.IdentifyCardNumber) || a.IdentifyCardNumber.ToLower().Contains(searchCondition.IdentifyCardNumber.ToLower()))
                                 && (string.IsNullOrEmpty(searchCondition.Address) || a.Address.ToLower().Contains(searchCondition.Address.ToLower()))
                                 && (searchCondition.Gender == -1 || a.Gender == searchCondition.Gender)
                                 && (string.IsNullOrEmpty(searchCondition.JobTitleId) || a.JobTitleId.Equals(searchCondition.JobTitleId))
                                 && (string.IsNullOrEmpty(searchCondition.DepartmentId) || a.DepartmentId.Equals(searchCondition.DepartmentId))
                                 select new EmployeeSearchResult()
                                 {
                                     EmployeeId = a.EmployeeId,
                                     DepartmentName = b.Name,
                                     JobTitleName = c.Name,
                                     FaceId = a.FaceId,
                                     Name = a.Name,
                                     Code = a.Code,
                                     DateOfBirth = a.DateOfBirth,
                                     Gender = a.Gender == Constants.Male ? "Nam" : "Nữ",
                                     InComeDate = a.InComeDate,
                                     OutComeDate = a.OutComeDate,
                                     Address = a.Address,
                                     Description = a.Description,
                                     IdentifyCardNumber = a.IdentifyCardNumber,
                                     LinkImage = a.LinkImage,
                                     EmpSelect = false,
                                 }).AsQueryable();
                if (searchCondition.DateFrom.HasValue)
                {
                    searchCondition.DateFrom = DateTimeUtils.ConvertDateFrom(searchCondition.DateFrom);
                    listmodel = listmodel.Where(r => r.InComeDate >= searchCondition.DateFrom);
                }
                if (searchCondition.DateTo.HasValue)
                {
                    searchCondition.DateTo = DateTimeUtils.ConvertDateTo(searchCondition.DateTo);
                    listmodel = listmodel.Where(r => r.InComeDate <= searchCondition.DateTo);
                }

                searchResult = listmodel.OrderBy(u => u.Name).ToList();
                foreach (var item in searchResult)
                {
                    item.Index = index++;
                    if (string.IsNullOrEmpty(item.LinkImage))
                    {
                        item.Avata = "";
                    }
                    else
                    {
                        img = item.LinkImage.Split(';')[0];
                        item.Avata = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + img;
                    }
                }
                return searchResult;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public EmployeeModel GetEmployeeById(string id)
        {
            db = new TimeAttendanceEntities();

            try
            {
                var result = (from a in db.Employee.AsNoTracking()
                              where (a.EmployeeId.Equals(id))
                              select new EmployeeModel()
                              {
                                  EmployeeId = a.EmployeeId,
                                  DepartmentId = a.DepartmentId,
                                  JobTitleId = a.JobTitleId,
                                  FaceId = a.FaceId,
                                  Name = a.Name,
                                  Code = a.Code,
                                  DateOfBirth = a.DateOfBirth,
                                  Gender = a.Gender.ToString(),
                                  InComeDate = a.InComeDate,
                                  OutComeDate = a.OutComeDate,
                                  Address = a.Address,
                                  Description = a.Description,
                                  IdentifyCardNumber = a.IdentifyCardNumber,
                                  LinkImage = a.LinkImage,
                                  LinkImageFaceId = a.LinkImageFaceId,

                              }).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public void CreateEmployee(EmployeeModel model)
        {

            var rs = "";
            var mess = "";
            var CreaterPertionErro = "";
            db = new TimeAttendanceEntities();
            var dateNow = DateTime.Now;
            var modelCheck = db.Employee.FirstOrDefault(r => r.Code.Equals(model.Code));
            if (modelCheck != null)
            {
                throw new BusinessException("Mã nhân viên này đã tồn tại!");
            }
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {

                    var result = Task.Run(async () => { return await FaceServiceHelper.CreatePersonAsync(NhomNV, model.Code); }).Result;
                    if (result != null)
                    {
                        this.PersonsInCurrentGroup.Add(new Person { Name = model.Code, PersonId = result.PersonId });
                        Employee modelCreate = new Employee()
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            Name = model.Name,
                            Code = model.Code,
                            FaceId = model.FaceId,
                            DateOfBirth = model.DateOfBirth,
                            Gender = int.Parse(model.Gender),
                            InComeDate = model.InComeDate,
                            OutComeDate = model.OutComeDate,
                            IdentifyCardNumber = model.IdentifyCardNumber,
                            LinkImage = model.LinkImage,
                            DepartmentId = model.DepartmentId,
                            JobTitleId = model.JobTitleId,
                            Description = model.Description,
                            Address = model.Address,
                            LinkImageFaceId = "",
                            CreateBy = model.CreateBy,
                            CreateDate = dateNow,
                            UpdateBy = model.CreateBy,
                            UpdateDate = dateNow,
                        };
                        modelCreate.FaceId = result.PersonId.ToString();

                        if (modelCreate.FaceId.Equals("1") || modelCreate.FaceId.Equals("-1"))
                        {
                            CreaterPertionErro = "-1";
                            throw new BusinessException("Tạo PersonId lỗi vui lòng thử lại");
                        }
                        db.Employee.Add(modelCreate);

                        //luu Log lich su
                        string decription = "Thêm mới nhân viên có mã-tên là : " + modelCreate.Code + "-" + modelCreate.Name;
                        LogBusiness.SaveLogEvent(db, model.CreateBy, decription, null);

                        db.SaveChanges();
                        trans.Commit();


                        //Thêm nhật list nhân viên toàn cục
                        if (TimeAttendanceStatic.ListInfoEmployee == null || !TimeAttendanceStatic.ListInfoEmployee.Any())
                            TimeAttendanceStatic.ListInfoEmployee = new List<InfoEmployeeModel>();
                        var infoEmployee = GetInfoEmployee(modelCreate.EmployeeId);
                        if (infoEmployee != null)
                        {
                            TimeAttendanceStatic.ListInfoEmployee.Add(infoEmployee);
                        }

                        //up ảnh
                        List<ImageAnalyzer> args = new List<ImageAnalyzer>();
                        ImageAnalyzer img;
                        string imgUrl = "";
                        if (!string.IsNullOrEmpty(model.LinkImage))
                        {
                            var mang = model.LinkImage.Split(';').ToList();

                            foreach (var itemPath in mang)
                            {
                                if (!string.IsNullOrEmpty(itemPath))
                                {
                                    imgUrl =  itemPath;
                                    img = new ImageAnalyzer(imgUrl);
                                    img.LocalImagePath = imgUrl;
                                    args.Add(img);
                                }
                            }
                            //up ảnh
                            rs = OnImageSearchCompleted(args, result.PersonId, NhomNV);
                            //cập nhật lại faceid khi up ảnh
                            if (!rs.Equals("-1"))
                            {
                                var EmpNew = db.Employee.FirstOrDefault(u => u.EmployeeId.Equals(modelCreate.EmployeeId));
                                EmpNew.LinkImageFaceId = rs;
                                db.SaveChanges();
                            }
                            else
                            {
                                mess = "Ảnh quá lớn hoặc không thể nhận dạng được khuôn mặt";
                                throw new BusinessException(mess);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!rs.Equals("-1"))
                    {
                        trans.Rollback();
                        if (CreaterPertionErro.Equals("-1"))
                        {
                            throw new BusinessException("Tạo PersonId lỗi vui lòng thử lại");
                        }
                    }
                    if (rs.Equals("-1"))
                    {
                        throw new ErrorException(mess, ex.InnerException);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        public string UpdateEmployee(EmployeeModel model, string imageLink)
        {
            var rs = "";
            var mess = "";
            string LinkImageFaceIdOld = "";
            string link = "";
            db = new TimeAttendanceEntities();
            var modelEdit = db.Employee.FirstOrDefault(r => r.EmployeeId.Equals(model.EmployeeId));
            if (modelEdit == null)
            {
                throw new BusinessException(ErrorMessage.ERR007);
            }
            else
            {
                link = modelEdit.LinkImage;
                LinkImageFaceIdOld = modelEdit.LinkImageFaceId;
            }
            var modelCheck = db.Employee.FirstOrDefault(r => r.Code.Equals(model.Code) && !r.EmployeeId.Equals(model.EmployeeId));
            if (modelCheck != null)
            {
                throw new BusinessException("Mã nhân viên này đã tồn tại!");
            }
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var NameOld = modelEdit.Name;

                    modelEdit.Name = model.Name;
                    modelEdit.Code = model.Code;
                    modelEdit.DateOfBirth = model.DateOfBirth;
                    modelEdit.Gender = int.Parse(model.Gender);
                    modelEdit.InComeDate = model.InComeDate;
                    modelEdit.OutComeDate = model.OutComeDate;
                    modelEdit.IdentifyCardNumber = model.IdentifyCardNumber;
                    modelEdit.LinkImage = model.LinkImage;
                    modelEdit.DepartmentId = model.DepartmentId;
                    modelEdit.JobTitleId = model.JobTitleId;
                    modelEdit.Description = model.Description;
                    modelEdit.Address = model.Address;
                    modelEdit.LinkImageFaceId = model.LinkImageFaceId;
                    modelEdit.UpdateBy = model.CreateBy;
                    modelEdit.UpdateDate = DateTime.Now;
                    var decription = "Cập nhật thông tin nhân viên có tên ban đầu là:  " + NameOld + " thành " + model.Name; ;
                    LogBusiness.SaveLogEvent(db, model.CreateBy, decription, null);
                    db.SaveChanges();
                    trans.Commit();

                    //Cập nhật list nhân viên toàn cục
                    if (TimeAttendanceStatic.ListInfoEmployee == null || !TimeAttendanceStatic.ListInfoEmployee.Any())
                        TimeAttendanceStatic.ListInfoEmployee = new List<InfoEmployeeModel>();
                    var infoEmployee = GetInfoEmployee(model.EmployeeId);
                    if (infoEmployee != null)
                    {
                        TimeAttendanceStatic.ListInfoEmployee = TimeAttendanceStatic.ListInfoEmployee.Where(r => !r.EmployeeId.Equals(model.EmployeeId)).ToList();
                        TimeAttendanceStatic.ListInfoEmployee.Add(infoEmployee);
                    }

                    //up ảnh
                    #region
                    List<ImageAnalyzer> args = new List<ImageAnalyzer>();
                    ImageAnalyzer img;
                    string imgUrl = "";
                    var mang = imageLink.Split(';').ToList();
                    if (!string.IsNullOrEmpty(imageLink))
                    {
                        foreach (var itemPath in mang)
                        {
                            if (!string.IsNullOrEmpty(itemPath))
                            {

                                imgUrl =  itemPath;
                                img = new ImageAnalyzer(imgUrl);
                                img.LocalImagePath = imgUrl;
                                args.Add(img);
                            }
                        }

                        rs = OnImageSearchCompleted(args, Guid.Parse(modelEdit.FaceId), NhomNV);
                        string LinkImageFaceIdNew = "";
                        if (!string.IsNullOrEmpty(rs) )
                        {
                            //cập nhật lại faceid ảnh
                            if (!rs.Equals("-1"))
                            {
                                if (string.IsNullOrEmpty(model.LinkImageFaceId))
                                {
                                    LinkImageFaceIdNew =  rs;
                                }
                                else
                                {
                                    LinkImageFaceIdNew = model.LinkImageFaceId + ";" + rs;
                                }

                                var EmpNew = db.Employee.FirstOrDefault(u => u.EmployeeId.Equals(modelEdit.EmployeeId));
                                EmpNew.LinkImageFaceId = LinkImageFaceIdNew;
                                db.SaveChanges();
                            }
                            else
                            {
                                mess = "Ảnh quá lớn hoặc không thể nhận dạng được khuôn mặt";
                                throw new BusinessException(mess);
                            }
                        }

                    }
                    #endregion

                    //xóa ảnh
                    //xử lý faceid ảnh mới và cũ
                    #region
                    List<string> imgRemote = new List<string>();
                    if (!string.IsNullOrEmpty(LinkImageFaceIdOld))
                    {
                        string[] linkmodel = model.LinkImageFaceId.Split(';');
                        var list = LinkImageFaceIdOld.Split(';').ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (!linkmodel.Contains(list[i]))
                            {
                                imgRemote.Add(list[i]);
                            }
                        }
                    }
                    RemoteImageApi(imgRemote, Guid.Parse(modelEdit.FaceId));
                    #endregion

                }
                catch (Exception ex)
                {
                    if (!rs.Equals("-1"))
                    {
                        trans.Rollback();
                    }
                    if (rs.Equals("-1"))
                    {
                        throw new ErrorException(mess, ex.InnerException);

                    }
                    else
                    {
                        throw ex;
                    }
                }
                return link;
            }
        }

        public string DeleteEmployee(string id, string CreateBy)
        {
            db = new TimeAttendanceEntities();
            string LinkImageFaceId = "";
            string PersonId = "";
            //Xóa nhóm
            var objectDelete = db.Employee.FirstOrDefault(r => r.EmployeeId.Equals(id));
            if (objectDelete == null)
            {
                throw new BusinessException(ErrorMessage.ERR007);
            }
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var timeCheck = db.TimeAttendanceLog.Where(u => u.EmployeeId.Equals(objectDelete.EmployeeId));
                    if (timeCheck.Count() > 0)
                    {
                        db.TimeAttendanceLog.RemoveRange(timeCheck);
                    }
                    var AtttimeCheck = db.AttendanceLog.Where(u => u.EmployeeId.Equals(objectDelete.EmployeeId));
                    if (AtttimeCheck.Count() > 0)
                    {
                        db.AttendanceLog.RemoveRange(AtttimeCheck);
                    }
                    PersonId = objectDelete.FaceId;
                    LinkImageFaceId = objectDelete.LinkImageFaceId;
                    var name = objectDelete.Name;
                    //Xóa nhóm quyền
                    db.Employee.Remove(objectDelete);
                    //luu Log lich su
                    string decription = "Xóa thông tin nhân viên có tên là: " + name;
                    LogBusiness.SaveLogEvent(db, CreateBy, decription, null);

                    db.SaveChanges();
                    trans.Commit();

                    if (TimeAttendanceStatic.ListInfoEmployee == null || !TimeAttendanceStatic.ListInfoEmployee.Any())
                        TimeAttendanceStatic.ListInfoEmployee = new List<InfoEmployeeModel>();
                    TimeAttendanceStatic.ListInfoEmployee = TimeAttendanceStatic.ListInfoEmployee.Where(r => !r.EmployeeId.Equals(id)).ToList();

                    Person ps = new Person();
                    ps.Name = objectDelete.Name;
                    ps.PersonId = System.Guid.Parse(objectDelete.FaceId);
                    Task.Run(async () => { await FaceServiceHelper.DeletePersonAsync(NhomNV, ps.PersonId); });
                    // await FaceServiceHelper.DeletePersonAsync(NhomNV, ps.PersonId);
                    this.PersonsInCurrentGroup.Remove(ps);
                    try
                    {
                        //xoa anh neu có
                        if (!string.IsNullOrEmpty(objectDelete.LinkImageFaceId))
                        {
                            var list = LinkImageFaceId.Split(';').ToList();
                            RemoteImageApi(list, Guid.Parse(PersonId));
                        }
                    }
                    catch (Exception)
                    { }
                    //thay đổi
                    Task.Run(async () =>
                    {
                        await FaceServiceHelper.TrainPersonGroupAsync(NhomNV);
                    });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
            return objectDelete.LinkImage;
        }
        public void RemoteImageApi(List<string> list, Guid PersonId)
        {
            try
            {
                //xoa anh neu có
                if (list != null && list.Count > 0)
                {
                    PersistedFace personFace;
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            personFace = new PersistedFace();
                            personFace.PersistedFaceId = Guid.Parse(item);
                            Task.Run(async () =>
                            {
                                await FaceServiceHelper.DeletePersonFaceAsync(NhomNV, PersonId, personFace.PersistedFaceId);
                            });
                            this.SelectedPersonFaces.Remove(personFace);
                        }
                    }
                    //thay đổi
                    Task.Run(async () =>
                    {
                        await FaceServiceHelper.TrainPersonGroupAsync(NhomNV);
                    });
                }
            }
            catch (Exception)
            { }
        }
        public string OnImageSearchCompleted(List<ImageAnalyzer> args, Guid PersonId, string PersonGroupId)
        {
            string rs = "";
            try
            {
                foreach (var item in args)
                {
                    AddPersistedFaceResult addResult;
                  //  using (FileStream stream = File.Open(item.ImageUrl, FileMode.Open))
                   // {
                        addResult = Task.Run(async () =>
                        {
                            return await FaceServiceHelper.AddPersonFaceAsync(
                                            PersonGroupId,
                                            PersonId,
                                            item.ImageUrl,
                                            userData: item.ImageUrl,
                                            targetFace: null);
                        }).Result;
                        if (string.IsNullOrEmpty(rs))
                        {
                            rs += addResult.PersistedFaceId.ToString();
                        }
                        else
                        {
                            rs += ";" + addResult.PersistedFaceId.ToString();
                        }
                    //}
                }
                //thay đổi
                Task.Run(async () =>
                {
                    await FaceServiceHelper.TrainPersonGroupAsync(PersonGroupId);
                });
            }
            catch (Exception e)
            {
                rs = "-1";
                // throw new BusinessException("Kích thước ảnh lớn quá mức cho phép");
            }
            return rs;
        }

        public string Training(List<AttendanceLogSearchResult> list)
        {

            string UrlHost = System.Configuration.ConfigurationManager.AppSettings["UrlHostImage"] + System.Configuration.ConfigurationManager.AppSettings["StorageContainer"] + "/";
            string rs = "0";
            try
            {
                //up ảnh
                List<ImageAnalyzer> args = new List<ImageAnalyzer>();
                ImageAnalyzer img;
                string imgUrl = "";
                List<AttendanceLogSearchResult> listByEmp = new List<AttendanceLogSearchResult>();
                var listEmpSelect = list.Select(ux => ux.EmployeeId).ToList();
                List<Employee> empList = db.Employee.Where(u => listEmpSelect.Contains(u.EmployeeId)).ToList();
                foreach (var item in empList)
                {
                    listByEmp = list.Where(u => u.EmployeeId.Equals(item.EmployeeId)).ToList();
                    foreach (var itemsub in listByEmp)
                    {
                        if (!string.IsNullOrEmpty(itemsub.ImageFace))
                        {
                            imgUrl = itemsub.ImageFace;
                            img = new ImageAnalyzer(UrlHost+imgUrl);
                            img.LocalImagePath = UrlHost+imgUrl;
                            args.Add(img);
                        }
                    }
                    if (listByEmp.Count > 0)
                    {
                        rs = OnImageSearchCompleted(args, Guid.Parse(item.FaceId), NhomNV);
                        if (rs != "-1")
                        {
                            rs = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rs = "-1";
            }
            return rs;
        }

        public SearchResultObject<TimeAttendanceLogSearchResult> SearchTimeAttendanceLog(TimeAttendanceLogSearchCondition searchCondition)
        {
            db = new TimeAttendanceEntities();

            SearchResultObject<TimeAttendanceLogSearchResult> searchResult = new SearchResultObject<TimeAttendanceLogSearchResult>();
            try
            {
                var listmodel = (from a in db.Employee.AsNoTracking()
                                 join b in db.TimeAttendanceLog.AsNoTracking() on a.EmployeeId equals b.EmployeeId
                                 select new TimeAttendanceLogSearchResult()
                                 {
                                     DepartmentName = a.EmployeeId,
                                     EmployeeName = a.Name,
                                     Date = b.Date,
                                     TimeIn = b.TimeIn,
                                     TimeOut = b.TimeOut,
                                     ImageIn = b.ImageIn,
                                     ImageOut = b.ImageOut,
                                     Total = b.Total,
                                     LateMinutes = b.LateMinutes,
                                     EarlyMinutes = b.EarlyMinutes
                                 }).AsQueryable();
                if (searchCondition.DateFrom.HasValue)
                {
                    searchCondition.DateFrom = DateTimeUtils.ConvertDateFrom(searchCondition.DateFrom);
                    listmodel = listmodel.Where(r => r.Date >= searchCondition.DateFrom);
                }
                if (searchCondition.DateTo.HasValue)
                {
                    searchCondition.DateTo = DateTimeUtils.ConvertDateTo(searchCondition.DateTo);
                    listmodel = listmodel.Where(r => r.Date <= searchCondition.DateTo);
                }
                searchResult.ListResult = listmodel.OrderBy(u => u.EmployeeName).ThenBy(u => u.Date).ToList();
                return searchResult;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public SearchResultObject<TimeAttendanceLogSearchResult> SearchTimeAttendanceLogDetail(TimeAttendanceLogSearchCondition searchCondition)
        {
            db = new TimeAttendanceEntities();
            SearchResultObject<TimeAttendanceLogSearchResult> searchResult = new SearchResultObject<TimeAttendanceLogSearchResult>();
            try
            {
                var culture = new System.Globalization.CultureInfo("vi-VN");
                // var day = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);

                var listmodel = (from a in db.Employee.AsNoTracking()
                                 where (string.IsNullOrEmpty(searchCondition.Code) || a.Code.ToLower().Contains(searchCondition.Code.ToLower()))
                                 && (string.IsNullOrEmpty(searchCondition.Name) || a.Name.ToLower().Contains(searchCondition.Name.ToLower()))
                                 && (string.IsNullOrEmpty(searchCondition.DepartmentId) || a.DepartmentId.Equals(searchCondition.DepartmentId))
                                 && (string.IsNullOrEmpty(searchCondition.JobTitleId) || a.JobTitleId.Equals(searchCondition.JobTitleId))
                                 join c in db.Department.AsNoTracking() on a.DepartmentId equals c.DepartmentId
                                 join b in db.TimeAttendanceLog.AsNoTracking() on a.EmployeeId equals b.EmployeeId
                                 join d in db.JobTitle.AsNoTracking() on a.JobTitleId equals d.JobTitleId
                                 select new TimeAttendanceLogSearchResult()
                                 {

                                     DepartmentName = c.Name,
                                     JobTitleName = d.Name,
                                     EmployeeCode = a.Code,
                                     EmployeeName = a.Name,
                                     Date = b.Date,
                                     TimeIn = b.TimeIn,
                                     TimeOut = b.TimeOut,
                                     ImageIn = b.ImageIn,
                                     ImageOut = b.ImageOut,
                                     Total = b.Total,
                                     LateMinutes = b.LateMinutes,
                                     EarlyMinutes = b.EarlyMinutes,
                                     ImageFaceIn = b.ImageFaceIn,
                                     ImageFaceOut = b.ImageFaceOut,
                                     //DayView = culture.DateTimeFormat.GetDayName(b.Date.DayOfWeek)
                                 }).AsQueryable();
                if (searchCondition.DateFrom.HasValue)
                {
                    searchCondition.DateFrom = DateTimeUtils.ConvertDateFrom(searchCondition.DateFrom);
                    listmodel = listmodel.Where(r => r.Date >= searchCondition.DateFrom);
                }
                if (searchCondition.DateTo.HasValue)
                {
                    searchCondition.DateTo = DateTimeUtils.ConvertDateTo(searchCondition.DateTo);
                    listmodel = listmodel.Where(r => r.Date <= searchCondition.DateTo);
                }
                searchResult.TotalItem = listmodel.Select(u => u.TimeIn).Count();

                if (searchCondition.Export == 1)
                {
                    searchResult.ListResult = SQLHelpper.OrderBy(listmodel, searchCondition.OrderBy, searchCondition.OrderType).ToList();
                    var rs = ExPort(searchResult.ListResult, searchCondition.DateFrom.Value, searchCondition.DateTo.Value);
                    searchResult.PathFile = rs;
                    searchResult.ListResult = searchResult.ListResult.Skip((searchCondition.PageNumber - 1) * searchCondition.PageSize)
             .Take(searchCondition.PageSize).ToList();
                }
                else
                {
                    searchResult.ListResult = SQLHelpper.OrderBy(listmodel, searchCondition.OrderBy, searchCondition.OrderType).Skip((searchCondition.PageNumber - 1) * searchCondition.PageSize)
              .Take(searchCondition.PageSize).ToList();
                }
                foreach (var item in searchResult.ListResult)
                {
                    item.DayView = culture.DateTimeFormat.GetDayName(item.Date.Value.DayOfWeek);
                }
                return searchResult;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public string ExPort(List<TimeAttendanceLogSearchResult> list, DateTime DateFrom, DateTime DateTo)
        {
            string pathExport = "";
            try
            {
                var culture = new System.Globalization.CultureInfo("vi-VN");
                // Khỏi tạo bảng excel
                ExcelEngine excelEngine = new ExcelEngine();

                IApplication application = excelEngine.Excel;

                IWorkbook workbook = application.Workbooks.Open(HttpContext.Current.Server.MapPath("/Template/Thongke.xlsx"));

                IWorksheet sheet = workbook.Worksheets[0];

                IRange rangeDateFrom = sheet.FindFirst("<DateFrom>", ExcelFindType.Text, ExcelFindOptions.MatchCase);
                rangeDateFrom.Text = rangeDateFrom.Text.Replace("<DateFrom>", (DateFrom.ToString("dd/MM/yyy")));
                IRange rangeDateTo = sheet.FindFirst("<DateTo>", ExcelFindType.Text, ExcelFindOptions.MatchCase);
                rangeDateTo.Text = rangeDateTo.Text.Replace("<DateTo>", (DateTo.ToString("dd/MM/yyy")));

                int total = list.Count;
                int index = 1;
                var listExport = (from a in list
                                  select new
                                  {
                                      Index = index++,
                                      date = a.Date.HasValue ? a.Date.Value.ToString("dd-MM-yyyy") : string.Empty,
                                      dayView = culture.DateTimeFormat.GetDayName(a.Date.Value.DayOfWeek),

                                      a.EmployeeCode,
                                      a.EmployeeName,
                                      a.JobTitleName,
                                      a.DepartmentName,
                                      datein = a.TimeIn.HasValue ? a.TimeIn.Value.ToString("HH:mm") : string.Empty,
                                      dateout = a.TimeOut.HasValue ? a.TimeOut.Value.ToString("HH:mm") : string.Empty,
                                      a.Total,
                                      a.EarlyMinutes,
                                      a.LateMinutes,

                                  }).ToList();

                IRange iRangeData = sheet.FindFirst("<Data>", ExcelFindType.Text, ExcelFindOptions.MatchCase);
                if (total == 0)
                {
                    iRangeData.Text = iRangeData.Text.Replace("<Data>", "");
                }
                else
                {
                    sheet.ImportData(listExport, iRangeData.Row, iRangeData.Column, false);
                }
                pathExport = "/Template/Export/" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "Thongke.xlsx";
                workbook.SaveAs(HttpContext.Current.Server.MapPath(pathExport));

            }
            catch (Exception)
            { }
            return pathExport;
        }

        /// <summary>
        /// Gét thông tin nhân viên theo id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        private InfoEmployeeModel GetInfoEmployee(string employeeId)
        {
            db = new TimeAttendanceEntities();
            try
            {
                var model = (from a in db.Employee.AsNoTracking()
                             where a.EmployeeId.Equals(employeeId)
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
                             }).FirstOrDefault();

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public SearchResultObject<TransactionLogSearchResult> TransactionLog(TimeAttendanceLogSearchCondition searchCondition)
        {
            db = new TimeAttendanceEntities();
            SearchResultObject<TransactionLogSearchResult> searchResult = new SearchResultObject<TransactionLogSearchResult>();
            try
            {
                var listmodel = (from a in db.TransactionLog.AsNoTracking()
                                 where (string.IsNullOrEmpty(searchCondition.CameraIPAdress) || a.CameraIPAdress.Contains(searchCondition.CameraIPAdress))
                                 && (string.IsNullOrEmpty(searchCondition.ClientIPAddress) || a.ClientIPAddress.Contains(searchCondition.ClientIPAddress))
                                 select new TransactionLogSearchResult()
                                 {
                                     TransactionLogId = a.TransactionLogId,
                                     ClientIPAddress = a.ClientIPAddress,
                                     CameraIPAdress = a.CameraIPAdress,
                                     Date = a.Date,
                                     CallDateTime = a.CallDateTime,
                                     ResponseDateTime = a.ResponseDateTime,
                                     ResponseTime = a.ResponseTime,
                                     ImageLink = a.ImageLink,
                                     StatusCode = a.StatusCode,
                                     StatusCodeView = a.StatusCode.Trim().Equals(Constants.RequestOk) ? "Thành công" : "Lỗi",
                                 }).AsQueryable();
                if (searchCondition.DateFrom.HasValue)
                {
                    // searchCondition.DateFrom = DateTimeUtils.ConvertDateFrom(searchCondition.DateFrom);
                    listmodel = listmodel.Where(r => r.Date >= searchCondition.DateFrom);
                }
                if (searchCondition.DateTo.HasValue)
                {
                    // searchCondition.DateTo = DateTimeUtils.ConvertDateTo(searchCondition.DateTo);
                    listmodel = listmodel.Where(r => r.Date <= searchCondition.DateTo);
                }
                if (!string.IsNullOrEmpty(searchCondition.StatusCode))
                {
                    if (searchCondition.StatusCode.Equals(Constants.RequestOk))
                    {
                        listmodel = listmodel.Where(r => r.StatusCode.Equals(searchCondition.StatusCode));
                    }
                    else
                    {
                        listmodel = listmodel.Where(r => !r.StatusCode.Equals(Constants.RequestOk));
                    }
                }
                //searchResult.ListResult = listmodel.OrderBy(u => u.Date).ToList();
                //searchResult.TotalItem = searchResult.ListResult.Count();

                searchResult.TotalItem = listmodel.Select(r => r.TransactionLogId).Count();
                searchResult.ListResult = SQLHelpper.OrderBy(listmodel, searchCondition.OrderBy, searchCondition.OrderType).Skip((searchCondition.PageNumber - 1) * searchCondition.PageSize)
               .Take(searchCondition.PageSize).ToList();
                if (searchResult.TotalItem != 0)
                {
                    searchResult.TotalItemOkCount = listmodel.Where(r => r.StatusCode.Equals(Constants.RequestOk)).Count();
                    searchResult.TotalItemOk = searchResult.TotalItemOkCount * 100 / searchResult.TotalItem;
                    searchResult.TotalItemNotOk = 100 - searchResult.TotalItemOk;
                }
                else
                {
                    searchResult.TotalItemOk = 0;
                    searchResult.TotalItemNotOk = 0;
                }

                return searchResult;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public SearchResultObject<MCRP3100HeatMap> DensityLog(TimeAttendanceLogSearchCondition searchCondition)
        {
            db = new TimeAttendanceEntities();
            SearchResultObject<MCRP3100HeatMap> searchResult = new SearchResultObject<MCRP3100HeatMap>();
            try
            {
                List<MCRP3100HeatMap> listMCRP3100HeatMap = new List<MCRP3100HeatMap>();
                int[] vehicleInStatistic = new int[25];
                DateTime monday;
                string dateToList = string.Empty;
                List<string> listDay = new List<string>();
                string[] listTime = { "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00" };

                #region[bỏ lấy data từ db]
                //var listmodel = (from a in db.TransactionLog.AsNoTracking()
                //                 select new TransactionLogSearchResult()
                //                 {
                //                     TransactionLogId = a.TransactionLogId,
                //                     ClientIPAddress = a.ClientIPAddress,
                //                     CameraIPAdress = a.CameraIPAdress,
                //                     Date = a.Date,
                //                     CallDateTime = a.CallDateTime,
                //                     ResponseDateTime = a.ResponseDateTime,
                //                     ResponseTime = a.ResponseTime,
                //                     ImageLink = a.ImageLink,
                //                     StatusCode = a.StatusCode,
                //                 }).AsQueryable();
                //if (searchCondition.DateFrom.HasValue)
                //{
                //    searchCondition.DateFrom = DateTimeUtils.ConvertDateFrom(searchCondition.DateFrom);
                //    listmodel = listmodel.Where(r => r.Date >= searchCondition.DateFrom);
                //}
                //if (searchCondition.DateTo.HasValue)
                //{
                //    searchCondition.DateTo = DateTimeUtils.ConvertDateTo(searchCondition.DateTo);
                //    listmodel = listmodel.Where(r => r.Date <= searchCondition.DateTo);
                //}

                //var ListResult = listmodel.ToList();
                //var dateCount = searchCondition.DateTo - searchCondition.DateFrom;
                //for (int i = 0; i < dateCount.Value.Days + 1; i++)
                //{
                //    monday = searchCondition.DateFrom.Value.AddDays(i);
                //    dateToList = monday.Date.ToString("dd/MM/yyyy");
                //    listDay.Add(dateToList);
                //    var list = ListResult.Where(r => r.Date.Value.Date == monday.Date);
                //    for (int j = 1; j < 25; j++)
                //    {

                //        vehicleInStatistic[j] = list.Where(r => r.Date.Value.Hour == j - 1).Count();
                //        listMCRP3100HeatMap.Add(new MCRP3100HeatMap
                //        {
                //            x = j - 1,
                //            y = i,
                //            VehicleInStatistic = vehicleInStatistic[j]
                //        });

                //    }
                //}
                #endregion
                #region[add cache cứng]
                //18-11-04
                //var day1 = searchCondition.DateFrom.Value.Day;
                //if (day1 == 18 || day1==11 || day1==4)
                //{
                //    var DateFrom_To1 = searchCondition.DateFrom.Value.ToString("ddMMyyyy") + "_" + searchCondition.DateTo.Value.ToString("ddMMyyyy");
                //    DensityCache cacheModel1 = new DensityCache() ;
                //    cacheModel1.DateFrom_ToKey = DateFrom_To1;
                //    cacheModel1.ListResult = listMCRP3100HeatMap;
                //    cacheModel1.listDay = listDay;
                //    cacheModel1.listTime = listTime.ToList();
                //    var redis1 = RedisService<DensityCache>.GetInstance();
                //    redis1.Add(DateFrom_To1, cacheModel1);
                //}
                #endregion

                #region[lấy từ cache redis]
                var RedisConnection = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"];
                var DateFrom_ToKey = "AT:Density:" + searchCondition.DateFrom.Value.ToString("ddMMyyyy") + "_" + searchCondition.DateTo.Value.ToString("ddMMyyyy");
                var redis = RedisService<DensityCache>.GetInstance(RedisConnection);
                DensityCache cacheModel;
                cacheModel = redis.Get<DensityCache>(DateFrom_ToKey);
                if (cacheModel != null)
                {
                    searchResult.ListResult = cacheModel.ListResult; ;
                    searchResult.listDay = cacheModel.listDay;
                    searchResult.listTime = cacheModel.listTime;
                }
                else
                {
                    var dateCount = searchCondition.DateTo - searchCondition.DateFrom;
                    for (int i = 0; i < dateCount.Value.Days + 1; i++)
                    {
                        monday = searchCondition.DateFrom.Value.AddDays(i);
                        dateToList = monday.Date.ToString("dd/MM/yyyy");
                        listDay.Add(dateToList);
                        for (int j = 1; j < 25; j++)
                        {
                            listMCRP3100HeatMap.Add(new MCRP3100HeatMap
                            {
                                x = j - 1,
                                y = i,
                                VehicleInStatistic = 0
                            });

                        }
                    }
                    searchResult.ListResult = listMCRP3100HeatMap;
                    searchResult.listDay = listDay;
                    searchResult.listTime = listTime.ToList();
                }
                #endregion

                return searchResult;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }


    }
}
