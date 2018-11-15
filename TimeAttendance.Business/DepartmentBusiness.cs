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
    public class DepartmentBusiness
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();

        public List< DepartmentSearchResult> GetListDepartment(DepartmentSearchCondition model)
        {
            try
            {
                var listDepartment = (from d in db.Department.AsNoTracking()
                                      where d.Name.Contains(model.Name)
                                      orderby d.Name
                                      select new DepartmentSearchResult()
                                      {
                                          DepartmentId = d.DepartmentId,
                                          Name = d.Name,
                                          Description = d.Description
                                      }).ToList();

                return listDepartment;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public DepartmentSearchResult GetDepartmentInfo(string departmentId)
        {
            try
            {
                var department = (from p in db.Department.AsNoTracking()
                                  where p.DepartmentId.Equals(departmentId)
                                  select new DepartmentSearchResult()
                                  {
                                      DepartmentId = p.DepartmentId,
                                      Name = p.Name,
                                      Description = p.Description,
                                  }).FirstOrDefault();
                return department;
            }
            catch(Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }

        }

        public void AddDepartment(DepartmentModel model)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    Department add = new Department()
                    {
                        DepartmentId = Guid.NewGuid().ToString(),
                        Name = model.Name,
                        Description = model.Description,
                        CreateDate = DateTime.Now,
                        CreateBy = model.CreateBy,
                    };
                    db.Department.Add(add);

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

        public int UpdateDepartment(DepartmentModel model)
        {
            using (var trans = db.Database.BeginTransaction())
            {

                try
                {
                    var data = db.Department.FirstOrDefault(u => u.DepartmentId.Equals(model.DepartmentId));
                    if (data != null)
                    {
                        data.Name = model.Name;
                        data.Description = model.Description;

                        trans.Commit();
                        db.SaveChanges();
                        return Constants.OK;
                    }
                    else
                    {
                        return Constants.NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }
        }

        public int DeleteDepartment(string departmentId)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var check = db.Employee.FirstOrDefault(r => r.DepartmentId.Equals(departmentId));
                    if (check != null)
                    {
                        return  Constants.USING;
                    }
                    var data = db.Department.FirstOrDefault(u => u.DepartmentId.Equals(departmentId));
                    if (data != null)
                    {
                        db.Department.Remove(data);
                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        return  Constants.NOT_FOUND;
                    }

                    return  Constants.OK;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
                }
            }

        }

    }
}
