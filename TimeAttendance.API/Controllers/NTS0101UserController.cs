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
using System.Threading.Tasks;
using TimeAttendance.Storage;

namespace TimeAttendance.API.Controllers
{
    [RoutePrefix("api/NTS0101")]
    public class NTS0101UserController : ApiController
    {
        // Log4net for NTS0101UserController
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(NTS0101UserController));
        private readonly UserBusiness _userBusiness = new UserBusiness();

        [Route("SearchUser")]
        [HttpPost]
        public HttpResponseMessage SearchUser(UserSearchCondition userSearchConditionEntity)
        {
            try
            {
                SearchResultObject<UserSearchResult> result = _userBusiness.SearchUser(userSearchConditionEntity);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("SearchUserTimeAttendance")]
        [HttpPost]
        public HttpResponseMessage SearchUserTimeAttendance(UserSearchCondition userSearchConditionEntity)
        {
            try
            {
                SearchResultObject<UserSearchResult> result = _userBusiness.SearchUserTimeAttendance(userSearchConditionEntity);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("SearchUserCustomer")]
        [HttpPost]
        public HttpResponseMessage SearchUserCustomer(UserSearchCondition userSearchConditionEntity)
        {
            try
            {
                SearchResultObject<UserSearchResult> result = _userBusiness.SearchUserCustomer(userSearchConditionEntity);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("CreateUser")]
        [HttpPost]
        //[NTSAuthorize(AllowFeature = "SY0002")]
        public HttpResponseMessage CreateUser()
        {
            var modelJson = HttpContext.Current.Request.Form["Model"];
            UserModel model = JsonConvert.DeserializeObject<UserModel>(modelJson);
            HttpFileCollection hfc = HttpContext.Current.Request.Files;

            string imageLink = string.Empty;
            try
            {
                if (hfc.Count > 0)
                {
                    imageLink = Task.Run(async () =>
                    {
                        return await AzureStorageUploadFiles.GetInstance().UploadPhotoAsync(hfc[0], hfc[0].FileName, Constants.FolderImageUser);
                    }).Result;
                    // imageLink = UploadFileServer.UploadFile(hfc[0], Constants.FolderImageUser);
                }

                model.ImageLink = imageLink;
                _userBusiness.CreateUser(model);

                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);

                //Xóa file trên server nếu cập nhật lỗi
                UploadFileServer.DeleteFile(imageLink);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("GetUserById")]
        [HttpPost]
        public HttpResponseMessage GetUserById(string id)
        {
            try
            {
                UserModel result = _userBusiness.GetUserById(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [Route("GetUserCustomerById")]
        [HttpPost]
        public HttpResponseMessage GetUserCustomerById(string id)
        {
            try
            {
                UserModel result = _userBusiness.GetUserById(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("UpdateUser")]
        [HttpPost]
        //[NTSAuthorize(AllowFeature = "SY0002")]
        public HttpResponseMessage UpdateUser()
        {
            var modelJson = HttpContext.Current.Request.Form["Model"];
            UserModel model = JsonConvert.DeserializeObject<UserModel>(modelJson);
            HttpFileCollection hfc = HttpContext.Current.Request.Files;

            string imageLink = string.Empty;
            string imageLinkOld = string.Empty;
            try
            {
                if (hfc.Count > 0)
                {
                    imageLink = Task.Run(async () =>
                    {
                        return await AzureStorageUploadFiles.GetInstance().UploadPhotoAsync(hfc[0], hfc[0].FileName, Constants.FolderImageUser);
                    }).Result;
                    //Upload file lên server
                    //  imageLink = UploadFileServer.UploadFile(hfc[0], Constants.FolderImageUser);
                    imageLinkOld = model.ImageLink;
                }


                model.ImageLink = !string.IsNullOrEmpty(imageLink) ? imageLink : model.ImageLink;
                _userBusiness.UpdateUser(model);

                if (!string.IsNullOrEmpty(imageLinkOld))
                {
                    //Xóa file cũ nếu cập nhật thành công
                    UploadFileServer.DeleteFile(imageLinkOld);
                }

                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                //Xóa file upload nếu cập nhật lỗi
                UploadFileServer.DeleteFile(imageLink);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("DeleteUser")]
        [HttpPost]
        //[NTSAuthorize(AllowFeature = "SY0002")]
        public HttpResponseMessage DeleteUser(string id, UserModel model)
        {
            try
            {
                _userBusiness.DeleteUser(id, model);

                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("GetSelectGroupAll")]
        [HttpPost]
        public HttpResponseMessage GetSelectGroupAll()
        {
            try
            {
                SearchResultObject<GroupEntity> searchResutl = new UserBusiness().GetSelectGroupAll();
                return Request.CreateResponse(HttpStatusCode.OK, searchResutl.ListResult);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("GetSelectGroup")]
        [HttpPost]
        public HttpResponseMessage GetSelectGroup()
        {
            try
            {
                SearchResultObject<GroupEntity> searchResutl = new UserBusiness().GetSelectGroup();
                return Request.CreateResponse(HttpStatusCode.OK, searchResutl.ListResult);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("UpdateStatusUser")]
        [HttpPost]
        //[NTSAuthorize(AllowFeature = "SY0002")]
        public HttpResponseMessage UpdateStatusUser(string id, UserModel model)
        {
            try
            {
                _userBusiness.UpdateStatusUser(id, model);

                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("ResetPass")]
        [HttpPost]
        //[NTSAuthorize(AllowFeature = "SY0002")]
        public HttpResponseMessage ResetPass(string id, UserModel model)
        {
            try
            {
                _userBusiness.ResetPass(id);

                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id nhóm quyền</param>
        /// <returns></returns>
        [Route("GetFunction")]
        [HttpPost]
        public HttpResponseMessage GetFunction(string id)
        {
            try
            {
                List<FunctionModel> listFunction = _userBusiness.GetFunction(id);

                return Request.CreateResponse(HttpStatusCode.OK, listFunction);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id người dùng</param>
        /// <returns></returns>
        [Route("GetFunctionByUser")]
        [HttpPost]
        public HttpResponseMessage GetFunctionByUser(string id)
        {
            try
            {
                List<FunctionModel> listFunction = _userBusiness.GetFunctionByUser(id);
                return Request.CreateResponse(HttpStatusCode.OK, listFunction);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
