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

namespace TimeAttendance.Business
{
    public class GroupUserBusiness
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();
        private AuthContext authContext = new AuthContext();

        public GroupUserBusiness()
        {

        }

        public SearchResultObject<GroupSearchResult> SearchGroupUser(GroupUserSearchCondition searchCondition)
        {
            db = new TimeAttendanceEntities();

            SearchResultObject<GroupSearchResult> searchResult = new SearchResultObject<GroupSearchResult>();
            try
            {
                List<string> userDelete = db.User.AsNoTracking().Where(r => r.DeleteFlg == Constants.DeleteTrue).Select(p => p.UserId).ToList();
                var listmodel = (from a in db.Group.AsNoTracking()
                                 join b in db.UserGroup.AsNoTracking() on a.GroupId equals b.GroupId into ab
                                 select new GroupSearchResult()
                                 {
                                     GroupId = a.GroupId,
                                     Name = a.Name,
                                     HomePage = a.HomePage,
                                     Status = a.Status,
                                     Description = a.Description,
                                     CreateBy = a.CreateBy,
                                     CreateDate = a.CreateDate,
                                     UpdateBy = a.UpdateBy,
                                     UpdateDate = a.UpdateDate,
                                     CountUser = ab.Where(r => !userDelete.Any(w => w.Equals(r.UserId))).Count(),
                                 }).AsQueryable();
                if (!string.IsNullOrEmpty(searchCondition.Name))
                {
                    listmodel = listmodel.Where(r => r.Name.ToLower().Contains(searchCondition.Name.ToLower()));
                }
                if (!string.IsNullOrEmpty(searchCondition.Description))
                {
                    listmodel = listmodel.Where(r => r.Description.ToLower().Contains(searchCondition.Description.ToLower()));
                }
                if (searchCondition.Status.HasValue)
                {
                    listmodel = listmodel.Where(r => r.Status.HasValue && r.Status.Value == searchCondition.Status.Value);
                }

                searchResult.TotalItem = listmodel.Select(r => r.GroupId).Count();
                searchResult.ListResult = SQLHelpper.OrderBy(listmodel, searchCondition.OrderBy, searchCondition.OrderType).Skip((searchCondition.PageNumber - 1) * searchCondition.PageSize)
                            .Take(searchCondition.PageSize)
                            .ToList();

                return searchResult;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public void CreateGroup(GroupModel model)
        {
            db = new TimeAttendanceEntities();

            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    Group modelCreate = new Group()
                    {
                        GroupId = Guid.NewGuid().ToString(),
                        Name = model.Name,
                        HomePage = model.HomePage,
                        Status = Constants.UnLock,
                        Description = model.Description,
                        CreateBy = model.CreateBy,
                        CreateDate = DateTime.Now,
                    };
                    db.Group.Add(modelCreate);

                    //Thêm dánh sách quyền cho nhóm
                    List<GroupPermission> listPermission = new List<GroupPermission>();
                    GroupPermission modelPermission;
                    if (model.ListPermission != null && model.ListPermission.Count() > 0)
                    {
                        foreach (var item in model.ListPermission)
                        {
                            modelPermission = new GroupPermission()
                            {
                                GroupPermissionId = Guid.NewGuid().ToString(),
                                GroupId = modelCreate.GroupId,
                                FunctionId = item.FunctionId,
                            };
                            listPermission.Add(modelPermission);
                        }
                        db.GroupPermission.AddRange(listPermission);
                    }

                    //luu Log lich su
                    string decription = "Thêm mới nhóm quyền có tên là : " + modelCreate.Name;
                    LogBusiness.SaveLogEvent(db, model.UserId, decription, null);

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


        public GroupModel GetGroupUserById(string id)
        {
            db = new TimeAttendanceEntities();

            var item = db.Group.Find(id);
            if (item == null)
            {
                throw new BusinessException(ErrorMessage.ERR005);
            }
            try
            {
                GroupModel result = new GroupModel()
                {
                    GroupId = item.GroupId,
                    Name = item.Name,
                    Status = item.Status,
                    HomePage = item.HomePage,
                    Description = item.Description,
                    CreateBy = item.CreateBy,
                    CreateDate = item.CreateDate,
                    UpdateBy = item.UpdateBy,
                    UpdateDate = item.UpdateDate,
                };

                List<FunctionModel> listFunction = new List<FunctionModel>();

                listFunction = (from a in db.GroupPermission.AsNoTracking()
                                where a.GroupId.Equals(id)
                                select new FunctionModel
                                {
                                    FunctionId = a.FunctionId,
                                    Select = true,

                                }).ToList();

                result.ListPermission = listFunction;

                return result;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public void UpdateGroupUser(GroupModel model)
        {
            db = new TimeAttendanceEntities();
            var modelEdit = db.Group.Where(r => r.GroupId.Equals(model.GroupId)).FirstOrDefault();
            var groupNameOld = string.Empty;

            if (modelEdit == null)
            {
                throw new BusinessException(ErrorMessage.ERR005);
            }
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    groupNameOld = modelEdit.Name;

                    modelEdit.Name = model.Name;
                    modelEdit.HomePage = model.HomePage;
                    modelEdit.Description = model.Description;
                    modelEdit.UpdateBy = model.UpdateBy;
                    modelEdit.UpdateDate = DateTime.Now;

                    //Thêm dánh sách quyền cho nhóm
                    List<GroupPermission> listPermission = new List<GroupPermission>();
                    GroupPermission modelPermission;
                    //Xóa quyền cũ
                    var listPermissionDelete = db.GroupPermission.Where(r => r.GroupId.Equals(model.GroupId));
                    db.GroupPermission.RemoveRange(listPermissionDelete);

                    //Thêm quyền mới
                    if (model.ListPermission != null && model.ListPermission.Count() > 0)
                    {
                        foreach (var item in model.ListPermission)
                        {
                            modelPermission = new GroupPermission()
                            {
                                GroupPermissionId = Guid.NewGuid().ToString(),
                                GroupId = model.GroupId,
                                FunctionId = item.FunctionId,
                            };
                            listPermission.Add(modelPermission);
                        }
                        db.GroupPermission.AddRange(listPermission);
                    }

                    //luu Log lich su
                    string decription = String.Empty;
                    if (groupNameOld.ToLower() == modelEdit.Name.ToLower())
                    {
                        decription = "Cập nhật thông tin nhóm quyền có tên là: " + groupNameOld;
                    }
                    else
                    {
                        decription = "Cập nhật thông tin nhóm quyền có tên ban đầu là:  " + groupNameOld + " thành " + model.Name; ;
                    }
                    LogBusiness.SaveLogEvent(db, model.UserId, decription, null);

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

        public void DeleteGroupUser(string id, GroupModel model)
        {
            db = new TimeAttendanceEntities();

            //Xóa nhóm
            var objectDelete = db.Group.Where(r => r.GroupId.Equals(id));
            var nameGroup = string.Empty;
            if (objectDelete == null)
            {
                throw new BusinessException(ErrorMessage.ERR005);
            }

            //Xóa thành viên trong bảng liên kết
            var userGroupsDelete = db.UserGroup.AsNoTracking().Where(r => r.GroupId.Equals(id)).ToList();
            if (userGroupsDelete.Count() > 0)
            {
                throw new BusinessException(ErrorMessage.ERR006);
            }

            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    nameGroup = objectDelete.FirstOrDefault().Name;
                    //Xóa quyền của nhóm
                    var groupPermissionsDelete = db.GroupPermission.Where(r => r.GroupId.Equals(id));
                    db.GroupPermission.RemoveRange(groupPermissionsDelete);

                    //Xóa nhóm quyền
                    db.Group.RemoveRange(objectDelete);

                    //luu Log lich su
                    string decription = "Xóa thông tin nhóm quyền có tên là: " + nameGroup;
                    LogBusiness.SaveLogEvent(db, model.UserId, decription, null);

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

        public void UpdateStatusGroup(string id, GroupModel model)
        {
            db = new TimeAttendanceEntities();

            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var objectUpdate = db.Group.Find(id);
                    if (objectUpdate != null)
                    {
                        //Cập nhật trạng thái nhóm
                        objectUpdate.Status = (objectUpdate.Status.HasValue && objectUpdate.Status.Value == Constants.Lock ? Constants.UnLock : Constants.Lock);

                        //Cập nhật trạng thái thành viên trong nhóm
                        var userGroupsUpdate = db.UserGroup.Where(r => r.GroupId.Equals(id));
                        var listUser = db.User;
                        User userUpdate;
                        foreach (var item in userGroupsUpdate)
                        {
                            userUpdate = listUser.Where(r => r.UserId.Equals(item.UserId)).FirstOrDefault();
                            if (userUpdate != null)
                            {
                                userUpdate.Status = objectUpdate.Status;
                            }

                            //Xóa Tokens khi khóa tài khoản
                            if (objectUpdate.Status == Constants.Lock)
                            {
                                var deleteAuth = authContext.RefreshTokens.Where(r => r.Subject.Equals(userUpdate.Name));
                                authContext.RefreshTokens.RemoveRange(deleteAuth);
                                authContext.SaveChanges();
                            }
                        }
                    }

                    //luu Log lich su
                    string decription = "Cập nhật trạng thái nhóm có tên là: " + objectUpdate.Name;
                    LogBusiness.SaveLogEvent(db, model.UserId, decription, null);

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

        public List<FunctionModel> GetAllFunction()
        {
            db = new TimeAttendanceEntities();

            try
            {
                List<FunctionModel> listFunction = new List<FunctionModel>();

                var listResult = (from a in db.GroupFunction.AsNoTracking()
                                  join b in db.Function.AsNoTracking() on a.GroupFunctionId equals b.GroupFunctionId into ab
                                  orderby a.Name ascending
                                  select new
                                  {
                                      a.GroupFunctionId,
                                      a.Code,
                                      a.Name,
                                      a.Description,
                                      ListFunction = ab.ToList(),
                                  }).ToList();

                FunctionModel modelAdd;
                int index = 1;
                foreach (var item in listResult)
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
                    if (item.ListFunction != null)
                    {
                        int indexF = 1;
                        foreach (var itemFunc in item.ListFunction)
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
                    }
                    index++;
                }
                return listFunction;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public List<UserSearchResult> GetAllMemberInGroup(string groupId)
        {
            db = new TimeAttendanceEntities();

            try
            {
                var listmodel = (from a in db.User.AsNoTracking()
                                 where a.DeleteFlg == Constants.DeleteFalse
                                 join c in db.UserGroup.AsNoTracking() on a.UserId equals c.UserId into ac
                                 from acv in ac.DefaultIfEmpty()
                                 where acv.GroupId.Equals(groupId)
                                 orderby a.Name ascending
                                 select new UserSearchResult()
                                 {
                                     UserId = a.UserId,
                                     Name = a.Name,
                                     FullName = a.FullName,
                                     BirthDay = a.BirthDay,
                                     Agency = a.Agency,
                                     Email = a.Email,
                                     Role = a.Role,
                                     PhoneNumber = a.PhoneNumber,
                                     Description = a.Description,
                                     GroupId = acv != null ? acv.GroupId : string.Empty
                                 }).ToList();

                return listmodel;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }
    }
}
