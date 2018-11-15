// <copyright company="nhantinsoft.vn">
// Author: Vũ Văn Văn
// Created Date: 10/08/2016 17:08
// </copyright>
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Web;
using TimeAttendance.Business;
using TimeAttendance.Model.SearchResults;
using TimeAttendance.Model;
using TimeAttendance.Model.SearchCondition;

namespace TimeAttendance.API.Controllers
{
    [RoutePrefix("api/NTS0208")]
    public class NTS0208UserEventLogController : ApiController
    {
        // Log4net for PCTP0101UserController
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(NTS0208UserEventLogController));

        [Route("SearchUserEventLog")]
        [HttpPost]
        public HttpResponseMessage SearchUserEventLog(UserEventLogSearchCondition model, string saveOption)
        {
            try
            {
                SearchResultObject<UserEventLogSearchResult> result = EventLogBusiness.GetInstance().SearchUserEventLog(model, saveOption);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}

