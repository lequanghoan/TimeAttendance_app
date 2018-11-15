using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TimeAttendance.Business;
using TimeAttendance.Model;
using TimeAttendance.Model.Repositories;

namespace TimeAttendance.API.Controllers
{
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();
        private DashboardBusiness dashboardBusiness = new DashboardBusiness();

        [Route("GetDataDashboard")]
        [HttpPost]
        public HttpResponseMessage GetDataDashboard()
        {
            try
            {
                DashboardResultModel modelResult = dashboardBusiness.GetDataDashboard();

                return Request.CreateResponse(HttpStatusCode.OK, modelResult);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
