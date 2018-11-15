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
    public class JobTitleBusiness
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();

        public List<JobTitleSearchResult> GetListJobTitle(JobTitleSearchCondition model)
        {
            try
            {
                var listJobTitle = (from d in db.JobTitle.AsNoTracking()
                                      where d.Name.Contains(model.Name)
                                      orderby d.Name
                                      select new JobTitleSearchResult()
                                      {
                                          JobTitleId = d.JobTitleId,
                                          Name = d.Name,
                                          Description = d.Description
                                      }).ToList();

                return listJobTitle;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }
        }

        public JobTitleSearchResult GetJobTitleInfo(string JobTitleId)
        {
            try
            {
                var JobTitle = (from p in db.JobTitle.AsNoTracking()
                                  where p.JobTitleId.Equals(JobTitleId)
                                  select new JobTitleSearchResult()
                                  {
                                      JobTitleId = p.JobTitleId,
                                      Name = p.Name,
                                      Description = p.Description,
                                  }).FirstOrDefault();
                return JobTitle;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }

        }

        public void AddJobTitle(JobTitleModel model)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    JobTitle add = new JobTitle()
                    {
                        JobTitleId = Guid.NewGuid().ToString(),
                        Name = model.Name,
                        Description = model.Description,
                        CreateDate = DateTime.Now,
                        CreateBy = model.CreateBy,
                    };
                    db.JobTitle.Add(add);

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

        public int UpdateJobTitle(JobTitleModel model)
        {
            using (var trans = db.Database.BeginTransaction())
            {

                try
                {
                    var data = db.JobTitle.FirstOrDefault(u => u.JobTitleId.Equals(model.JobTitleId));
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

        public int DeleteJobtitle(string jobTitleId)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var check = db.Employee.FirstOrDefault(r => r.DepartmentId.Equals(jobTitleId));
                    if (check != null)
                    {
                        return Constants.USING;
                    }
                    var data = db.JobTitle.FirstOrDefault(u => u.JobTitleId.Equals(jobTitleId));
                    if (data != null)
                    {
                        db.JobTitle.Remove(data);
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

    }
}
