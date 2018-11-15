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
    [RoutePrefix("api/NoAttendanceLog")]
    public class NoAttendanceLogController : ApiController
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();
        private NoAttendanceLogBusiness attendanceLogBusiness = new NoAttendanceLogBusiness();

        [Route("GetListNoAttendanceLog")]
        [HttpPost]
        public HttpResponseMessage GetListNoAttendanceLog(NoAttendanceLogSearchCondition model)
        {
            try
            {
                var result = attendanceLogBusiness.GetListNoAttendanceLog(model);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("ExportExcel")]
        [HttpPost]
        public HttpResponseMessage ExportExcel(NoAttendanceLogSearchCondition model)
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
    }
}
