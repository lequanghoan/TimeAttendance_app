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
    [RoutePrefix("api/AttendanceLog")]
    public class AttendanceLogController : ApiController
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();
        private AttendanceLogBusiness attendanceLogBusiness = new AttendanceLogBusiness();

        [Route("GetListAttendanceLog")]
        [HttpPost]
        public HttpResponseMessage GetListAttendanceLog(AttendanceLogSearchCondition model)
        {
            try
            {
                var listResult = attendanceLogBusiness.GetListAttendanceLog(model);
                listResult.PathFile = System.Configuration.ConfigurationManager.AppSettings["UrlHostImage"] + System.Configuration.ConfigurationManager.AppSettings["StorageContainer"] + "/";

                return Request.CreateResponse(HttpStatusCode.OK, listResult);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //Lấy danh sách nhân viên theo phòng ban
        [Route("GetListEmployee")]
        [HttpPost]
        public HttpResponseMessage GetListEmployee(string departmentId)
        {
            try
            {
                var listResult = attendanceLogBusiness.GetListEmployee( departmentId);

                return Request.CreateResponse(HttpStatusCode.OK, listResult);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("GetListDepartment")]
        [HttpPost]
        public HttpResponseMessage GetListDepartment()
        {
            try
            {
                var listResult = attendanceLogBusiness.GetListDepartment();

                return Request.CreateResponse(HttpStatusCode.OK, listResult);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("UpdateAttendanceLog")]
        [HttpPost]
        public HttpResponseMessage UpdateAttendanceLog(AttendanceLogSearchResult model)
        {
            try
            {
                int result = attendanceLogBusiness.UpdateAttendanceLog(model);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("GetAttendanceLogInfo")]
        [HttpPost]
        public HttpResponseMessage GetAttendanceLogInfo(AttendanceLogSearchResult model)
        {
            try
            {
                var result = attendanceLogBusiness.GetAttendanceLogInfo(model);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("DeleteAttendanceLog")]
        [HttpPost]
        public HttpResponseMessage DeleteAttendanceLog(string attendanceLogId)
        {
            try
            {
                int result = attendanceLogBusiness.DeleteAttendanceLog(attendanceLogId);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [Route("ExportExcel")]
        [HttpPost]
        public HttpResponseMessage ExportExcel(AttendanceLogSearchCondition model)
        {
            try
            {
                AttendanceLogSearchResultObject result = attendanceLogBusiness.ExportExcel(model);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [Route("Training")]
        [HttpPost]
        public HttpResponseMessage Training(List<AttendanceLogSearchResult> model)
        {
            try
            {
               var rs= new EmployeeBusiness().Training(model);
                return Request.CreateResponse(HttpStatusCode.OK, rs);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [Route("GetAvataEmp")]
        [HttpPost]
        public HttpResponseMessage GetAvataEmp(string AttendanceLogId)
        {
            try
            {
                var result = attendanceLogBusiness.GetAvataEmp(AttendanceLogId);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

    }
}
