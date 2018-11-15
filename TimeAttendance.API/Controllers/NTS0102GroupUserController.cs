// <copyright company="nhantinsoft.vn">
// Author: Vũ Văn Văn
// Created Date: 09/08/2016 15:08
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
using TimeAttendance.Model.SearchCondition;
using TimeAttendance.Model.SearchResults;
using TimeAttendance.Business;

namespace TimeAttendance.API.Controllers
{
    [RoutePrefix("api/NTS0102")]
    public class NTS0102GroupUserController : ApiController
    {
        // Log4net for PCTP0102GroupUserController
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(NTS0102GroupUserController));
        private readonly GroupUserBusiness _groupUserBusiness = new GroupUserBusiness();

        [Route("SearchGroupUser")]
        [HttpPost]
        public HttpResponseMessage SearchGroupUser(GroupUserSearchCondition userSearchConditionEntity, [FromUri] int pageSize, [FromUri] int pageNumber)
        {
            try
            {
                userSearchConditionEntity.PageSize = pageSize;
                userSearchConditionEntity.PageNumber = pageNumber;
                SearchResultObject<GroupSearchResult> result = _groupUserBusiness.SearchGroupUser(userSearchConditionEntity);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("CreateGroupUser")]
        [HttpPost]
        //[NTSAuthorize(AllowFeature = "SY0001")]
        public HttpResponseMessage CreateGroupUser(GroupModel model)
        {
            try
            {
                _groupUserBusiness.CreateGroup(model);

                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("GetGroupUserById")]
        [HttpPost]
        public HttpResponseMessage GetGroupUserById(string id)
        {
            try
            {
                GroupModel result = _groupUserBusiness.GetGroupUserById(id);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("UpdateGroupUser")]
        [HttpPost]
        //[NTSAuthorize(AllowFeature = "SY0001")]
        public HttpResponseMessage UpdateGroupUser(GroupModel model)
        {

            try
            {
                _groupUserBusiness.UpdateGroupUser(model);

                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("DeleteGroupUser")]
        [HttpPost]
        //[NTSAuthorize(AllowFeature = "SY0001")]
        public HttpResponseMessage DeleteGroupUser(string id, GroupModel model)
        {
            try
            {
                _groupUserBusiness.DeleteGroupUser(id, model);
                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("UpdateStatusGroup")]
        [HttpPost]
        //[NTSAuthorize(AllowFeature = "SY0001")]
        public HttpResponseMessage UpdateStatusGroup(string id, GroupModel model)
        {
            try
            {
                _groupUserBusiness.UpdateStatusGroup(id, model);
                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("GetFunction")]
        [HttpPost]
        public HttpResponseMessage GetFunction()
        {
            try
            {
                List<FunctionModel> listFunction = _groupUserBusiness.GetAllFunction();

                return Request.CreateResponse(HttpStatusCode.OK, listFunction);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("ListMemberGroup")]
        [HttpPost]
        public HttpResponseMessage ListMemberGroup(string id)
        {
            try
            {
                List<UserSearchResult> listmodel = _groupUserBusiness.GetAllMemberInGroup(id);

                return Request.CreateResponse(HttpStatusCode.OK, listmodel);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}

