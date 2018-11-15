using TimeAttendance.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TimeAttendance.Model.Repositories;
using TimeAttendance.Utils;
using TimeAttendance.Business;
using TimeAttendance.Model;

namespace TimeAttendance.API.Controllers
{
    [RoutePrefix("api/JobTitle001")]
    public class JobTitle001Controller : ApiController
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();
        private JobTitleBusiness jobTitleBusiness = new JobTitleBusiness();

        [Route("GetListJobTitle")]
        [HttpPost]
        public HttpResponseMessage GetListJobTitle(JobTitle001Model model)
        {
            try
            {
                var listJobTitle = (from d in db.JobTitle.AsNoTracking()
                                    where d.Name.Contains(model.Name)
                                    orderby d.Name
                                    select new JobTitle001Model()
                                    {
                                        JobTitleId = d.JobTitleId,
                                        Name = d.Name,
                                        Description = d.Description
                                    }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, listJobTitle);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public HttpResponseMessage GetListJobTitle(JobTitleSearchCondition model)
        {
            try
            {
                var listResult = jobTitleBusiness.GetListJobTitle(model);
                return Request.CreateResponse(HttpStatusCode.OK, listResult);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        //Lấy thông tin vé tháng
        [Route("GetJobTitleInfo")]
        [HttpPost]

        public HttpResponseMessage GetJobTitleInfo(string jobTitleId)
        {
            try
            {
                var result = jobTitleBusiness.GetJobTitleInfo(jobTitleId);
                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, Constants.NOT_FOUND);
                }
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("AddJobTitle")]
        [HttpPost]

        public HttpResponseMessage AddJobTitle(JobTitleModel model)
        {
            try
            {
                jobTitleBusiness.AddJobTitle(model);

                return Request.CreateResponse(HttpStatusCode.OK, Constants.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("UpdateJobTitle")]
        [HttpPost]
        public HttpResponseMessage UpdateJobTitle(JobTitle001Model model)
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
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, Constants.NOT_FOUND);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, Constants.OK);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        public HttpResponseMessage UpdateJobTitle(JobTitleModel model)
        {
            try
            {
                int result = jobTitleBusiness.UpdateJobTitle(model);
                if (result == Constants.OK)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Constants.OK);
                }
                else if (result == Constants.NOT_FOUND)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Constants.NOT_FOUND);
                }
                return Request.CreateResponse(HttpStatusCode.OK, Constants.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("DeleteJobTitle")]
        [HttpPost]
        public HttpResponseMessage DeleteJobTitle(string jobTitleId)
        {
            try
            {
                int result = jobTitleBusiness.DeleteJobtitle(jobTitleId);
                if (result == Constants.NOT_FOUND)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Constants.NOT_FOUND);
                }
                if (result == Constants.USING)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Constants.USING);
                }
                if (result == Constants.OK)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Constants.OK);
                }
                return Request.CreateResponse(HttpStatusCode.OK, Constants.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
