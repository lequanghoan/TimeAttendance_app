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
    [RoutePrefix("api/Department001")]
    public class DashboardHomeController : ApiController
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();
        private DepartmentBusiness departmentBusiness = new DepartmentBusiness();

        [Route("GetListDepartment")]
        [HttpPost]
        public HttpResponseMessage GetListDepartment(DepartmentSearchCondition model)
        {
            try
            {
                var listResult = departmentBusiness.GetListDepartment(model);

                return Request.CreateResponse(HttpStatusCode.OK, listResult);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        //Lấy thông tin vé tháng
        [Route("GetDepartmentInfo")]
        [HttpPost]
        public HttpResponseMessage GetDepartmentInfo(string departmentId)
        {
            try
            {
                var result = departmentBusiness.GetDepartmentInfo(departmentId);
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

        [Route("AddDepartment")]
        [HttpPost]
        public HttpResponseMessage AddDepartment(DepartmentModel model)
        {
            try
            {
                departmentBusiness.AddDepartment(model);

                return Request.CreateResponse(HttpStatusCode.OK, Constants.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("UpdateDepartment")]
        [HttpPost]
        public HttpResponseMessage UpdateDepartment(DepartmentModel model)
        {
            try
            {
                int result = departmentBusiness.UpdateDepartment(model);
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

        [Route("DeleteDepartment")]
        [HttpPost]
        public HttpResponseMessage DeleteDepartment(string departmentId)
        {
            try
            {
                int result = departmentBusiness.DeleteDepartment(departmentId);
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
