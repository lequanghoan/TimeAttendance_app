// <copyright company="nhantinsoft.vn">
// Author: Vũ Văn Văn
// Created Date: 09/08/2016 12:08
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
using Newtonsoft.Json;
using TimeAttendance.Model;
using TimeAttendance.Model.SearchResults;
using TimeAttendance.Business;
using TimeAttendance.Model.SearchCondition;
using TimeAttendance.Utils;

namespace TimeAttendance.API.Controllers
{
    [RoutePrefix("api/NTSCombobox")]
    public class NTSComboboxController : ApiController
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(NTS0101UserController));
        private readonly ComboboxBusiness _userBusiness = new ComboboxBusiness();

        [Route("GetAllDepartment")]
        [HttpPost]
        public HttpResponseMessage GetAllDepartment()
        {
            try
            {
                var searchResutl = _userBusiness.GetAllDepartment();
                return Request.CreateResponse(HttpStatusCode.OK, searchResutl);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [Route("GetAllJobTitle")]
        [HttpPost]
        public HttpResponseMessage GetAllJobTitle()
        {
            try
            {
                var searchResutl = _userBusiness.GetAllJobTitle();
                return Request.CreateResponse(HttpStatusCode.OK, searchResutl);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("GetConfigResult")]
        [HttpPost]
        public HttpResponseMessage GetConfigResult()
        {
            try
            {
                var searchResutl = _userBusiness.GetConfigResult();
                return Request.CreateResponse(HttpStatusCode.OK, searchResutl);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [Route("UpdateConfigResult")]
        [HttpPost]
        public HttpResponseMessage UpdateConfigResult(ConfigResult con)
        {
            try
            {
                _userBusiness.UpdateConfigResult(con);
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [Route("ImportEmployee")]
        [HttpGet]
        public HttpResponseMessage ImportEmployee()
        {

            try
            {
                _userBusiness.ImportEmployee();
                return Request.CreateResponse(HttpStatusCode.OK, "OK");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
