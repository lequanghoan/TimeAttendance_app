using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Model;
using TimeAttendance.Utils;
using TimeAttendance.Model.SearchCondition;
using TimeAttendance.Model.SearchResults;
using System.Configuration;
using TimeAttendance.Model.Repositories;
using TimeAttendance.Caching;

namespace TimeAttendance.Business
{
    public class UserBusiness
    {
        string RedisConnection = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"];

        private TimeAttendanceEntities db;
        private AuthContext authContext = new AuthContext();

        public UserBusiness()
        {

        }

        public LoginEntity Login(string userName, string password)
        {
            db = new TimeAttendanceEntities();
            LoginEntity loginEntity = new LoginEntity();
            try
            {
                var userLogin = (from a in db.User.AsNoTracking()
                                 where a.Name.Equals(userName) && a.DeleteFlg == 0
                                 //join b in db.UserGroups.AsNoTracking() on a.UserId equals b.UserId into ab
                                 //from abv in ab.DefaultIfEmpty()
                                 select new
                                 {
                                     a.UserId,
                                    // a.UnitId,
                                     a.Name,
                                     a.FullName,
                                     a.BirthDay,
                                    // a.Agency,
                                     a.Email,
                                     a.Role,
                                     a.PhoneNumber,
                                     a.Password,
                                     a.PasswordHash,
                                     a.Status,
                                     a.Type,
                                     a.Description,
                                     a.ImageLink,
                                     a.CreateBy,
                                     a.CreateDate,
                                     a.UpdateBy,
                                     a.UpdateDate,
                                     a.IsAdmin,
                                     //abv.GroupId,

                                 }).FirstOrDefault();
                if (userLogin != null)
                {
                    if (userLogin.Status == Constants.Lock)
                    {
                        //Tài khoản bị khóa. Lên hệ quản trị để kích hoạt lại
                        loginEntity.ResponseCode = -6;
                    }
                    else
                    {
                        var securityStamp = PasswordUtil.ComputeHash(password + userLogin.Password);
                        if (userLogin.PasswordHash.Equals(securityStamp))
                        {
                            UserEntity userEntity = new UserEntity()
                            {
                                UserName = userLogin.Name,
                                UserId = userLogin.UserId,
                                FullName = userLogin.FullName,
                                Role = "1",
                                //UnitId = userLogin.UnitId,
                                //GroupId = userLogin.GroupId,
                                ImageLink = userLogin.ImageLink,
                                //Agency = userLogin.Agency,
                                Type = userLogin.Type,
                                IsAdmin = userLogin.IsAdmin.ToString(),
                                securityKey = PasswordUtil.CreatePasswordHash(),

                            };
                            userEntity.ListPermission = new List<string>();
                            userEntity.ListPermission = (from c in db.UserPermission.AsNoTracking()
                                                         where c.UserId.Equals(userLogin.UserId)
                                                         join d in db.Function.AsNoTracking() on c.FunctionId equals d.FunctionId
                                                         select d.Code).ToList<string>();

                            userEntity.HomePage = (from r in db.Group.AsNoTracking()
                                                   join a in db.UserGroup on r.GroupId equals a.GroupId
                                                   where a.UserId.Equals(userEntity.UserId)
                                                   select r.HomePage).FirstOrDefault();

                            loginEntity.UserInfor = userEntity;

                            LogBusiness.SaveLogLogin(db, userEntity.UserId);
                        }
                        else
                        {
                            // Mật khẩu không đúng
                            loginEntity.ResponseCode = -5;
                        }
                    }
                }
                else
                {
                    // tài khoản không có trong hệ thống
                    loginEntity.ResponseCode = -4;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

            return loginEntity;
        }

        public SearchResultObject<UserSearchResult> SearchUser(UserSearchCondition searchCondition)
        {
            db = new TimeAttendanceEntities();
            SearchResultObject<UserSearchResult> searchResult = new SearchResultObject<UserSearchResult>();
            try
            {
                var listmodel = (from a in db.User.AsNoTracking()
                                 where a.DeleteFlg == Constants.DeleteFalse && (string.IsNullOrEmpty(searchCondition.Type) || a.Type.Equals(searchCondition.Type))
                                 join c in db.UserGroup.AsNoTracking() on a.UserId equals c.UserId into ac
                                 from acv in ac.DefaultIfEmpty()
                                 join d in db.Group.AsNoTracking() on acv.GroupId equals d.GroupId into cd
                                 from cdv in cd.DefaultIfEmpty()
                                 select new UserSearchResult
                                 {
                                     UserId = a.UserId,
                                     UnitId = a.UnitId,
                                     Name = a.Name,
                                     FullName = a.FullName,
                                     BirthDay = a.BirthDay,
                                     Agency = a.Agency,
                                     Email = a.Email,
                                     Role = a.Role,
                                     PhoneNumber = a.PhoneNumber,
                                     Status = a.Status,
                                     Description = a.Description,
                                     ImageLink = a.ImageLink,
                                     CreateBy = a.CreateBy,
                                     CreateDate = a.CreateDate,
                                     UpdateBy = a.UpdateBy,
                                     UpdateDate = a.UpdateDate,
                                     GroupId = acv.GroupId,
                                     GroupName = cdv != null ? cdv.Name : string.Empty,
                                     Type = a.Type,
                                 }).AsQueryable();

                if (!string.IsNullOrEmpty(searchCondition.GroupId))
                {
                    listmodel = listmodel.Where(r => r.GroupId.Equals(searchCondition.GroupId));
                }
                if (!string.IsNullOrEmpty(searchCondition.Name))
                {
                    listmodel = listmodel.Where(r => r.Name.ToUpper().Contains(searchCondition.Name.ToUpper()));
                }
                if (!string.IsNullOrEmpty(searchCondition.FullName))
                {
                    listmodel = listmodel.Where(r => r.FullName.ToUpper().Contains(searchCondition.FullName.ToUpper()));
                }
                if (searchCondition.Status.HasValue)
                {
                    listmodel = listmodel.Where(r => r.Status.HasValue && r.Status.Value == searchCondition.Status.Value);
                }
                if (!string.IsNullOrEmpty(searchCondition.UnitId))
                {
                    listmodel = listmodel.Where(r => r.UnitId.Equals(searchCondition.UnitId));
                }
                if (!string.IsNullOrEmpty(searchCondition.Role))
                {
                    listmodel = listmodel.Where(r => r.Role.ToUpper().Contains(searchCondition.Role.ToUpper()));
                }
                if (searchCondition.Status.HasValue)
                {
                    listmodel = listmodel.Where(r => r.Status.HasValue && r.Status.Value == searchCondition.Status.Value);
                }

                searchResult.TotalItem = listmodel.Select(r => r.UserId).Count();
                var listResult = SQLHelpper.OrderBy(listmodel, searchCondition.OrderBy, searchCondition.OrderType).Skip((searchCondition.PageNumber - 1) * searchCondition.PageSize)
                            .Take(searchCondition.PageSize)
                            .ToList();
                searchResult.ListResult = listResult;

            }

            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
            return searchResult;
        }

        public SearchResultObject<UserSearchResult> SearchUserTimeAttendance(UserSearchCondition searchCondition)
        {
            db = new TimeAttendanceEntities();
            SearchResultObject<UserSearchResult> searchResult = new SearchResultObject<UserSearchResult>();
            try
            {
                var listmodel = (from a in db.User.AsNoTracking()
                                 where a.DeleteFlg == Constants.DeleteFalse && (string.IsNullOrEmpty(searchCondition.Type) || a.Type.Equals(searchCondition.Type))
                                 join c in db.UserGroup.AsNoTracking() on a.UserId equals c.UserId into ac
                                 from acv in ac.DefaultIfEmpty()
                                 join d in db.Group.AsNoTracking() on acv.GroupId equals d.GroupId into cd
                                 from cdv in cd.DefaultIfEmpty()
                                 select new UserSearchResult
                                 {
                                     UserId = a.UserId,
                                     UnitId = a.UnitId,
                                     Name = a.Name,
                                     FullName = a.FullName,
                                     BirthDay = a.BirthDay,
                                     Agency = a.Agency,
                                     Email = a.Email,
                                     Role = a.Role,
                                     PhoneNumber = a.PhoneNumber,
                                     Status = a.Status,
                                     Description = a.Description,
                                     ImageLink = a.ImageLink,
                                     CreateBy = a.CreateBy,
                                     CreateDate = a.CreateDate,
                                     UpdateBy = a.UpdateBy,
                                     UpdateDate = a.UpdateDate,
                                     GroupId = acv.GroupId,
                                     GroupName = cdv != null ? cdv.Name : string.Empty,
                                     Type = a.Type,
                                 }).AsQueryable();

                if (!string.IsNullOrEmpty(searchCondition.GroupId))
                {
                    listmodel = listmodel.Where(r => r.GroupId.Equals(searchCondition.GroupId));
                }
                if (!string.IsNullOrEmpty(searchCondition.Name))
                {
                    listmodel = listmodel.Where(r => r.Name.ToUpper().Contains(searchCondition.Name.ToUpper()));
                }
                if (!string.IsNullOrEmpty(searchCondition.FullName))
                {
                    listmodel = listmodel.Where(r => r.FullName.ToUpper().Contains(searchCondition.FullName.ToUpper()));
                }
                if (searchCondition.Status.HasValue)
                {
                    listmodel = listmodel.Where(r => r.Status.HasValue && r.Status.Value == searchCondition.Status.Value);
                }
                if (!string.IsNullOrEmpty(searchCondition.UnitId))
                {
                    listmodel = listmodel.Where(r => r.UnitId.Equals(searchCondition.UnitId));
                }
                if (!string.IsNullOrEmpty(searchCondition.Role))
                {
                    listmodel = listmodel.Where(r => r.Role.ToUpper().Contains(searchCondition.Role.ToUpper()));
                }
                if (searchCondition.Status.HasValue)
                {
                    listmodel = listmodel.Where(r => r.Status.HasValue && r.Status.Value == searchCondition.Status.Value);
                }

                searchResult.TotalItem = listmodel.Select(r => r.UserId).Count();
                var listResult = SQLHelpper.OrderBy(listmodel, searchCondition.OrderBy, searchCondition.OrderType).Skip((searchCondition.PageNumber - 1) * searchCondition.PageSize)
                            .Take(searchCondition.PageSize)
                            .ToList();
                searchResult.ListResult = listResult;

            }

            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
            return searchResult;
        }

        public SearchResultObject<UserSearchResult> SearchUserCustomer(UserSearchCondition searchCondition)
        {
            db = new TimeAttendanceEntities();
            SearchResultObject<UserSearchResult> searchResult = new SearchResultObject<UserSearchResult>();
            try
            {
                var listmodel = (from a in db.User.AsNoTracking()
                                 where a.DeleteFlg == Constants.DeleteFalse && (string.IsNullOrEmpty(searchCondition.Type) || a.Type.Equals(searchCondition.Type))
                                 join c in db.UserGroup.AsNoTracking() on a.UserId equals c.UserId into ac
                                 from acv in ac.DefaultIfEmpty()
                                 join d in db.Group.AsNoTracking() on acv.GroupId equals d.GroupId into cd
                                 from cdv in cd.DefaultIfEmpty()
                                 select new UserSearchResult
                                 {
                                     UserId = a.UserId,
                                     UnitId = a.UnitId,
                                     Name = a.Name,
                                     FullName = a.FullName,
                                     BirthDay = a.BirthDay,
                                     Agency = a.Agency,
                                     Email = a.Email,
                                     Role = a.Role,
                                     PhoneNumber = a.PhoneNumber,
                                     Status = a.Status,
                                     Description = a.Description,
                                     ImageLink = a.ImageLink,
                                     Address = a.Address,
                                     CreateBy = a.CreateBy,
                                     CreateDate = a.CreateDate,
                                     UpdateBy = a.UpdateBy,
                                     UpdateDate = a.UpdateDate,
                                     GroupId = acv.GroupId,
                                     GroupName = cdv != null ? cdv.Name : string.Empty,
                                     Type = a.Type,
                                 }).AsQueryable();

                if (!string.IsNullOrEmpty(searchCondition.GroupId))
                {
                    listmodel = listmodel.Where(r => r.GroupId.Equals(searchCondition.GroupId));
                }
                if (!string.IsNullOrEmpty(searchCondition.Name))
                {
                    listmodel = listmodel.Where(r => r.Name.ToUpper().Contains(searchCondition.Name.ToUpper()));
                }
                if (!string.IsNullOrEmpty(searchCondition.FullName))
                {
                    listmodel = listmodel.Where(r => r.FullName.ToUpper().Contains(searchCondition.FullName.ToUpper()));
                }
                if (searchCondition.Status.HasValue)
                {
                    listmodel = listmodel.Where(r => r.Status.HasValue && r.Status.Value == searchCondition.Status.Value);
                }
                if (!string.IsNullOrEmpty(searchCondition.UnitId))
                {
                    listmodel = listmodel.Where(r => r.UnitId.Equals(searchCondition.UnitId));
                }
                if (!string.IsNullOrEmpty(searchCondition.PhoneNumber))
                {
                    listmodel = listmodel.Where(r => r.PhoneNumber.ToUpper().Contains(searchCondition.PhoneNumber.ToUpper()));
                }
                if (searchCondition.Status.HasValue)
                {
                    listmodel = listmodel.Where(r => r.Status.HasValue && r.Status.Value == searchCondition.Status.Value);
                }

                searchResult.TotalItem = listmodel.Select(r => r.UserId).Count();
                var listResult = SQLHelpper.OrderBy(listmodel, searchCondition.OrderBy, searchCondition.OrderType).Skip((searchCondition.PageNumber - 1) * searchCondition.PageSize)
                            .Take(searchCondition.PageSize)
                            .ToList();
                searchResult.ListResult = listResult;

            }

            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
            return searchResult;
        }

        public void CreateUser(UserModel model)
        {
            db = new TimeAttendanceEntities();
            if (this.IsExistedUser(model.Name))
            {
                throw new BusinessException(ErrorMessage.ERR002);
            }
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    User modelCreate = new User()
                    {
                        UserId = Guid.NewGuid().ToString(),
                        Name = model.Name,
                        FullName = model.FullName,
                        BirthDay = model.BirthDay,
                        Agency = model.Agency,
                        Email = model.Email,
                        UnitId = model.UnitId,
                        Role = model.Role,
                        Type = model.Type,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        Status = Constants.UnLock,
                        Description = model.Description,
                        ImageLink = model.ImageLink,
                        CreateBy = model.CreateBy,
                        CreateDate = DateTime.Now,
                    };
                    modelCreate.Password = PasswordUtil.CreatePasswordHash();
                    modelCreate.PasswordHash = PasswordUtil.ComputeHash(Constants.PasswordDefault + modelCreate.Password);

                    db.User.Add(modelCreate);

                    if (!string.IsNullOrEmpty(model.GroupId))
                    {
                        UserGroup userGroup = new UserGroup()
                        {
                            UserGroupId = Guid.NewGuid().ToString(),
                            GroupId = model.GroupId,
                            UserId = modelCreate.UserId,
                        };
                        db.UserGroup.Add(userGroup);
                    }

                    //Thêm dánh sách quyền cho tài khoản
                    List<UserPermission> listPermission = new List<UserPermission>();
                    UserPermission modelPermission;
                    if (model.ListPermission != null && model.ListPermission.Count() > 0)
                    {
                        foreach (var item in model.ListPermission)
                        {
                            modelPermission = new UserPermission()
                            {
                                UserPermissionId = Guid.NewGuid().ToString(),
                                UserId = modelCreate.UserId,
                                FunctionId = item.FunctionId,
                            };
                            listPermission.Add(modelPermission);
                        }
                        db.UserPermission.AddRange(listPermission);
                    }

                    //luu Log lich su
                    string decription = "Thêm mới có tên là: " + model.Name;
                    LogBusiness.SaveLogEvent(db, model.LogUserId, decription, model.ViolationEventId);

                    db.SaveChanges();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }

        public UserModel GetUserById(string userId)
        {
            db = new TimeAttendanceEntities();
            try
            {
                var result = (from a in db.User.AsNoTracking()
                              where a.UserId.Equals(userId) && a.DeleteFlg == Constants.DeleteFalse
                              join c in db.UserGroup.AsNoTracking() on a.UserId equals c.UserId into ac
                              from acv in ac.DefaultIfEmpty()
                              orderby a.Name ascending
                              select new UserModel()
                              {
                                  UserId = a.UserId,
                                  Name = a.Name,
                                  FullName = a.FullName,
                                  BirthDay = a.BirthDay,
                                  Agency = a.Agency,
                                  Email = a.Email,
                                  UnitId = a.UnitId,
                                  Role = a.Role,
                                  PhoneNumber = a.PhoneNumber,
                                  Address = a.Address,
                                  Password = a.Password,
                                  PasswordHash = a.PasswordHash,
                                  Status = a.Status,
                                  Description = a.Description,
                                  ImageLink = a.ImageLink,
                                  CreateBy = a.CreateBy,
                                  CreateDate = a.CreateDate,
                                  UpdateBy = a.UpdateBy,
                                  UpdateDate = a.UpdateDate,
                                  GroupId = acv.GroupId,
                                  Type = a.Type,
                              }).FirstOrDefault();

                if (result == null)
                {
                    throw new BusinessException(ErrorMessage.ERR003);
                }

                result.ListPermission = (from a in db.UserPermission.AsNoTracking()
                                         where a.UserId.Equals(userId)
                                         select new FunctionModel
                                         {
                                             FunctionId = a.FunctionId,
                                             Select = true,
                                         }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public void UpdateUser(UserModel model)
        {
            db = new TimeAttendanceEntities();
            var modelEdit = db.User.Where(r => r.UserId.Equals(model.UserId) && r.DeleteFlg == Constants.DeleteFalse).FirstOrDefault();

            if (modelEdit == null)
            {
                throw new BusinessException(ErrorMessage.ERR003);
            }
            if (modelEdit.Name != model.Name)
            {
                if (this.IsExistedUser(model.Name))
                {
                    throw new BusinessException(ErrorMessage.ERR002);
                }
            }

            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var userNameOld = modelEdit.Name;

                    modelEdit.FullName = model.FullName;
                    modelEdit.BirthDay = model.BirthDay;
                    modelEdit.Agency = model.Agency;
                    modelEdit.Email = model.Email;
                    modelEdit.UnitId = model.UnitId;
                    modelEdit.Role = model.Role;
                    modelEdit.Type = model.Type;
                    modelEdit.PhoneNumber = model.PhoneNumber;
                    modelEdit.Address = model.Address;
                    modelEdit.Description = model.Description;
                    modelEdit.ImageLink = model.ImageLink;
                    modelEdit.UpdateBy = model.UpdateBy;
                    modelEdit.UpdateDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(model.GroupId))
                    {
                        var itemDelete = db.UserGroup.Where(r => r.UserId.Equals(model.UserId));
                        db.UserGroup.RemoveRange(itemDelete);
                        UserGroup userGroup = new UserGroup()
                        {
                            UserGroupId = Guid.NewGuid().ToString(),
                            GroupId = model.GroupId,
                            UserId = model.UserId,
                        };
                        db.UserGroup.Add(userGroup);
                    }

                    //Thêm dánh sách quyền cho tài khoản
                    List<UserPermission> listPermission = new List<UserPermission>();
                    UserPermission modelPermission;
                    //Xóa quyền cũ
                    var listPermissionDelete = db.UserPermission.Where(r => r.UserId.Equals(model.UserId));
                    db.UserPermission.RemoveRange(listPermissionDelete);

                    //Thêm quyền mới
                    if (model.ListPermission != null && model.ListPermission.Count() > 0)
                    {
                        foreach (var item in model.ListPermission)
                        {
                            modelPermission = new UserPermission()
                            {
                                UserPermissionId = Guid.NewGuid().ToString(),
                                UserId = model.UserId,
                                FunctionId = item.FunctionId,
                            };
                            listPermission.Add(modelPermission);
                        }
                        db.UserPermission.AddRange(listPermission);
                    }

                    //luu Log lich su
                    string decription = String.Empty;
                    if (userNameOld.ToLower() == modelEdit.Name.ToLower())
                    {
                        decription = "Cập nhật thông tin có tên là: " + userNameOld;
                    }
                    else
                    {
                        decription = "Cập nhật thông tin có tên ban đầu là:  " + userNameOld + " thành " + model.Name; ;
                    }
                    LogBusiness.SaveLogEvent(db, model.LogUserId, decription, null);

                    db.SaveChanges();
                    trans.Commit();

                    //xóa cache
                    var redis = RedisService<LoginCacheModel>.GetInstance(RedisConnection);
                    LoginCacheModel RefreshToken;
                    RefreshToken = redis.Get<LoginCacheModel>(Constants.ATLogin + userNameOld);
                    if (RefreshToken != null)
                    {
                        redis.Remove(Constants.ATLogin + userNameOld);
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }


        public void SelfUpdateInfo(UserModel model)
        {
            db = new TimeAttendanceEntities();
            var modelEdit = db.User.Where(r => r.UserId.Equals(model.UserId) && r.DeleteFlg == Constants.DeleteFalse).FirstOrDefault();

            if (modelEdit == null)
            {
                throw new BusinessException(ErrorMessage.ERR003);
            }
            if (modelEdit.Name != model.Name)
            {
                if (this.IsExistedUser(model.Name))
                {
                    throw new BusinessException(ErrorMessage.ERR002);
                }
            }

            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var userNameOld = modelEdit.Name;

                    modelEdit.FullName = model.FullName;
                    modelEdit.BirthDay = model.BirthDay;
                    modelEdit.Email = model.Email;
                    modelEdit.PhoneNumber = model.PhoneNumber;
                    modelEdit.Role = model.Role;
                    modelEdit.Agency = model.Agency;
                    modelEdit.ImageLink = model.ImageLink;
                    modelEdit.UpdateBy = model.UpdateBy;
                    modelEdit.UpdateDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(model.GroupId))
                    {
                        var itemDelete = db.UserGroup.Where(r => r.UserId.Equals(model.UserId));
                        db.UserGroup.RemoveRange(itemDelete);
                        UserGroup userGroup = new UserGroup()
                        {
                            UserGroupId = Guid.NewGuid().ToString(),
                            GroupId = model.GroupId,
                            UserId = model.UserId,
                        };
                        db.UserGroup.Add(userGroup);
                    }

                    string decription = "Cập nhật thông tin cá nhân";
                    LogBusiness.SaveLogEvent(db, model.LogUserId, decription, null);

                    db.SaveChanges();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }

        public void DeleteUser(string id, UserModel model)
        {
            db = new TimeAttendanceEntities();
            var objectDelete = db.User.Where(r => r.UserId.Equals(id) && r.DeleteFlg == Constants.DeleteFalse);

            if (objectDelete == null)
            {
                throw new BusinessException(ErrorMessage.ERR003);
            }
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var type = objectDelete.FirstOrDefault().Type;
                    var userName = objectDelete.FirstOrDefault().Name;

                    //Xóa Tokens
                    var deleteAuth = authContext.RefreshTokens.Where(r => r.Subject.Equals(userName));
                    authContext.RefreshTokens.RemoveRange(deleteAuth);
                    authContext.SaveChanges();

                    //Xóa logic tài khoản, gán lại cờ Delete = true
                    objectDelete.First().DeleteFlg = Constants.DeleteTrue;

                    //luu Log lich su
                    string decription = "Xóa thông tin có tên là: " + userName;
                    LogBusiness.SaveLogEvent(db, model.LogUserId, decription, null);

                    db.SaveChanges();
                    trans.Commit();

                    //xóa cache
                   // var RedisConnection = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"];
                    var redis = RedisService<LoginCacheModel>.GetInstance(RedisConnection);
                    LoginCacheModel RefreshToken;
                    RefreshToken = redis.Get<LoginCacheModel>(Constants.ATLogin + userName);
                    if (RefreshToken != null)
                    {
                        redis.Remove(Constants.ATLogin + userName);
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }

        /// <summary>
        /// Tất cả nhóm quyền
        /// </summary>
        /// <returns></returns>
        public SearchResultObject<GroupEntity> GetSelectGroupAll()
        {
            db = new TimeAttendanceEntities();
            SearchResultObject<GroupEntity> result = new SearchResultObject<GroupEntity>();
            try
            {
                var listResult = (from a in db.Group.AsNoTracking()
                                  where a.Status.HasValue
                                  orderby a.Name ascending
                                  select new GroupEntity()
                                  {
                                      GroupId = a.GroupId,
                                      Name = a.Name,
                                  }).ToList();

                result.ListResult = listResult;

                return result;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        /// <summary>
        /// Danh sách nhóm quyền đang kích hoạt
        /// </summary>
        /// <returns></returns>
        public SearchResultObject<GroupEntity> GetSelectGroup()
        {
            db = new TimeAttendanceEntities();
            SearchResultObject<GroupEntity> result = new SearchResultObject<GroupEntity>();
            try
            {
                var listResult = (from a in db.Group.AsNoTracking()
                                  where a.Status.HasValue && a.Status.Value == Constants.UnLock
                                  orderby a.Name ascending
                                  select new GroupEntity()
                                  {
                                      GroupId = a.GroupId,
                                      Name = a.Name,
                                  }).ToList();

                result.ListResult = listResult;

                return result;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public void UpdateStatusUser(string id, UserModel model)
        {
            db = new TimeAttendanceEntities();
            var objectUpdate = db.User.Find(id);

            if (objectUpdate == null)
            {
                throw new BusinessException(ErrorMessage.ERR002);
            }

            var group = (from a in db.Group.AsNoTracking()
                         where a.Status == Constants.Lock
                         join b in db.UserGroup.AsNoTracking() on a.GroupId equals b.GroupId
                         where b.UserId.Equals(id)
                         select a).ToList();

            if (group.Count() > 0 && objectUpdate.Status.HasValue && objectUpdate.Status.Value == Constants.Lock)
            {
                throw new BusinessException(ErrorMessage.ERR004);
            }


            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    objectUpdate.Status = (objectUpdate.Status.HasValue && objectUpdate.Status.Value == Constants.Lock ? Constants.UnLock : Constants.Lock);

                    //Xóa Tokens khi khóa tài khoản
                    if (objectUpdate.Status == Constants.Lock)
                    {
                        var deleteAuth = authContext.RefreshTokens.Where(r => r.Subject.Equals(objectUpdate.Name));
                        authContext.RefreshTokens.RemoveRange(deleteAuth);
                        authContext.SaveChanges();
                    }
                    db.SaveChanges();
                    trans.Commit();

                    //luu Log lich su
                    string decription = "Cập nhật trạng thái có tên là: " + objectUpdate.Name;
                    LogBusiness.SaveLogEvent(db, model.UserId, decription, null);
                    //xóa cache
                 //   var RedisConnection = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"];

                    var redis = RedisService<LoginCacheModel>.GetInstance(RedisConnection);
                    LoginCacheModel RefreshToken;
                    RefreshToken = redis.Get<LoginCacheModel>(Constants.ATLogin + objectUpdate.Name);
                    if (RefreshToken != null)
                    {
                        redis.Remove(Constants.ATLogin + objectUpdate.Name);
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }

        public void ResetPass(string userId)
        {
            db = new TimeAttendanceEntities();

            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var modelReset = db.User.Where(r => r.UserId.Equals(userId)).FirstOrDefault();
                    var name = string.Empty;
                    if (modelReset != null)
                    {
                        modelReset.Password = PasswordUtil.CreatePasswordHash();
                        modelReset.PasswordHash = PasswordUtil.ComputeHash(Constants.PasswordDefault + modelReset.Password);
                        name = modelReset.Name;
                    }

                    db.SaveChanges();
                    trans.Commit();

                    //luu Log lich su
                    string decription = "Lấy lại mật khẩu của user tên là: " + name;
                    LogBusiness.SaveLogEvent(db, userId, decription, null);
                    //xóa cache
                   // var RedisConnection = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"];

                    var redis = RedisService<LoginCacheModel>.GetInstance(RedisConnection);
                    LoginCacheModel RefreshToken;
                    RefreshToken = redis.Get<LoginCacheModel>(Constants.ATLogin + modelReset.Name);
                    if (RefreshToken != null)
                    {
                        redis.Remove(Constants.ATLogin + modelReset.Name);
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }

        public void ChangePass(UserModel model)
        {
            db = new TimeAttendanceEntities();

            var modelChange = db.User.Where(r => r.UserId.Equals(model.UserId)).FirstOrDefault();

            //Kiểm tra tồn tại
            if (modelChange == null)
            {
                throw new BusinessException(ErrorMessage.ERR003);
            }

            //Check mật khẩu cũ nhập
            var securityStamp = PasswordUtil.ComputeHash(model.OldPassword + modelChange.Password);
            if (!modelChange.PasswordHash.Equals(securityStamp))
            {
                throw new BusinessException(ErrorMessage.ERR007);
            }
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    modelChange.PasswordHash = PasswordUtil.ComputeHash(model.NewPassword + modelChange.Password);

                    //luu Log lich su
                    string decription = "Thay đổi mật khẩu cá nhân";
                    LogBusiness.SaveLogEvent(db, model.UserId, decription, null);

                    db.SaveChanges();
                    trans.Commit();
                    //xóa cache
                    var redis = RedisService<LoginCacheModel>.GetInstance(RedisConnection);
                    LoginCacheModel RefreshToken;
                    RefreshToken = redis.Get<LoginCacheModel>(Constants.ATLogin + modelChange.Name);
                    if (RefreshToken != null)
                    {
                        redis.Remove(Constants.ATLogin + modelChange.Name);
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }

        public List<FunctionModel> GetFunction(string id)
        {
            db = new TimeAttendanceEntities();

            try
            {
                List<FunctionModel> listFunction = new List<FunctionModel>();

                var listFunc = (from a in db.GroupPermission.AsNoTracking()
                                where a.GroupId.Equals(id)
                                join c in db.Function.AsNoTracking() on a.FunctionId equals c.FunctionId
                                orderby c.Name ascending
                                select new
                                {
                                    c.FunctionId,
                                    c.GroupFunctionId,
                                    c.Code,
                                    c.Name,
                                    c.Description
                                }).ToList();
                var listGroup = db.GroupFunction.AsNoTracking().ToList();

                FunctionModel modelAdd;
                int index = 1;
                foreach (var item in listGroup)
                {
                    var listFuncSelect = listFunc.Where(r => r.GroupFunctionId.Equals(item.GroupFunctionId));
                    if (listFuncSelect != null && listFuncSelect.Count() > 0)
                    {
                        modelAdd = new FunctionModel()
                        {
                            Index = index.ToString(),
                            FunctionId = item.GroupFunctionId,
                            GroupFunctionId = string.Empty,
                            Code = item.Code,
                            Name = item.Name,
                            Description = item.Description,
                            Select = false,
                        };
                        listFunction.Add(modelAdd);

                        int indexF = 1;
                        foreach (var itemFunc in listFuncSelect)
                        {
                            modelAdd = new FunctionModel()
                            {
                                Index = index.ToString() + "." + indexF.ToString(),
                                FunctionId = itemFunc.FunctionId,
                                GroupFunctionId = item.GroupFunctionId,
                                Code = itemFunc.Code,
                                Name = itemFunc.Name,
                                Description = itemFunc.Description,
                                Select = false,
                            };
                            listFunction.Add(modelAdd);
                            indexF++;
                        }

                        index++;
                    }
                }

                return listFunction;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public List<FunctionModel> GetFunctionByUser(string id)
        {
            try
            {
                db = new TimeAttendanceEntities();
                List<FunctionModel> listFunction = new List<FunctionModel>();
                var listFunc = (from a in db.UserPermission.AsNoTracking()
                                where a.UserId.Equals(id)
                                join c in db.Function.AsNoTracking() on a.FunctionId equals c.FunctionId
                                orderby c.Name ascending
                                select new
                                {
                                    c.FunctionId,
                                    c.GroupFunctionId,
                                    c.Code,
                                    c.Name,
                                    c.Description
                                }).ToList();
                var listGroup = db.GroupFunction.AsNoTracking().ToList();
                FunctionModel modelAdd;
                int index = 1;
                foreach (var item in listGroup)
                {
                    var listFuncSelect = listFunc.Where(r => r.GroupFunctionId.Equals(item.GroupFunctionId));
                    if (listFuncSelect != null && listFuncSelect.Count() > 0)
                    {
                        modelAdd = new FunctionModel()
                        {
                            Index = index.ToString(),
                            FunctionId = item.GroupFunctionId,
                            GroupFunctionId = string.Empty,
                            Code = item.Code,
                            Name = item.Name,
                            Description = item.Description,
                            Select = false,
                        };
                        listFunction.Add(modelAdd);

                        int indexF = 1;
                        foreach (var itemFunc in listFuncSelect)
                        {
                            modelAdd = new FunctionModel()
                            {
                                Index = index.ToString() + "." + indexF.ToString(),
                                FunctionId = itemFunc.FunctionId,
                                GroupFunctionId = item.GroupFunctionId,
                                Code = itemFunc.Code,
                                Name = itemFunc.Name,
                                Description = itemFunc.Description,
                                Select = false,
                            };
                            listFunction.Add(modelAdd);
                            indexF++;
                        }

                        index++;
                    }
                }

                return listFunction;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }


        /// <summary>
        /// Check tồn tại tài khoản
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private bool IsExistedUser(string userName)
        {
            db = new TimeAttendanceEntities();
            var listUser = db.User.AsNoTracking().Where(r => r.Name.Equals(userName));
            if (listUser.Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
