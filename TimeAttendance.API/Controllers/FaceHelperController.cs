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

namespace TimeAttendance.API.Controllers
{
    [RoutePrefix("api/FaceHelper")]
    public class FaceHelperController : ApiController
    {
        // Log4net for NTS0101UserController
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(FaceHelperController));
        private readonly FaceHelperBusiness faceHelperBusiness = new FaceHelperBusiness();

        [Route("DetectFace")]
        [HttpPost]
        public async Task<HttpResponseMessage> DetectFace()
        {
            string imageLink = string.Empty;
            try
            {
                var modelJson = HttpContext.Current.Request.Form["Model"];
                DetectFaceModel detectFaceModel = JsonConvert.DeserializeObject<DetectFaceModel>(modelJson);
                HttpFileCollection hfc = HttpContext.Current.Request.Files;
                if (hfc.Count > 0)
                {
                    detectFaceModel.ImageStream = hfc[0].InputStream;
                    detectFaceModel.LogImageLink = imageLink = UploadFileServer.UploadFileJPG(hfc[0], Constants.FolderLogAttendance);
                }
                var ipClient = GetIpClient.GetClientIpAddress(Request);
                detectFaceModel.ClientIPAddress = ipClient;
                DetectFaceResultModel result = await faceHelperBusiness.DetectFace(detectFaceModel);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                UploadFileServer.DeleteFile(imageLink);
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

      

    }
}
