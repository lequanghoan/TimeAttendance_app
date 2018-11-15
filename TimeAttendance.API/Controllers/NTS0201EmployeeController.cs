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
using System.Web.Hosting;
using System.Globalization;

using System.Threading.Tasks;
using TimeAttendance.Storage;

namespace TimeAttendance.API.Controllers
{
    [RoutePrefix("api/NTS0201")]
    public class NTS0201EmployeeController : ApiController
    {
        // Log4net for NTS0101UserController
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(NTS0101UserController));
        private readonly EmployeeBusiness _userBusiness = new EmployeeBusiness();

        [Route("SearchEmployee")]
        [HttpPost]
        [NTSAuthorize(AllowFeature = "CF0002")]
        public HttpResponseMessage SearchEmployee(EmployeeSearchCondition SearchCondition)
        {
            try
            {
                var result = _userBusiness.SearchEmployee(SearchCondition);
                //Task.Run(async () =>
                //{
                //    AzureStorageUploadFiles.GetInstance().DownloadPhotoAsync(result.ListResult[0].Avata);
                //});
                //Task.Run(async () =>
                //{
                //    AzureStorageUploadFiles.GetInstance().DeletePhotoAsync(result.ListResult[0].Avata);
                //});
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("CreateEmployee")]
        [HttpPost]
        public HttpResponseMessage CreateEmployee()
        {
            var modelJson = HttpContext.Current.Request.Form["Model"];
            EmployeeModel model = JsonConvert.DeserializeObject<EmployeeModel>(modelJson);
            HttpFileCollection hfc = HttpContext.Current.Request.Files;
            string pathFolder = "fileUpload/" + Constants.FolderEmployee + "/";
            string pathFolderServer = HostingEnvironment.MapPath("~/" + pathFolder);
            string imageLink = string.Empty;
            try
            {
                if (hfc.Count > 0)
                {
                    for (int i = 0; i < hfc.Count; i++)
                    {
                        if (i == 0)
                        {
                            // imageLink = Task.Run(async () => { return wait UploadPhotoAsync(hfc[i], hfc[i].FileName); }).Result;

                            imageLink = Task.Run(async () =>
                            {
                                return await AzureStorageUploadFiles.GetInstance().UploadPhotoAsync(hfc[i], hfc[i].FileName, Constants.FolderEmployee);
                            }).Result;
                        }
                        else
                        {
                            imageLink += ";" + Task.Run(async () =>
                            {
                                return await AzureStorageUploadFiles.GetInstance().UploadPhotoAsync(hfc[i], hfc[i].FileName, Constants.FolderEmployee);
                            }).Result;
                        }
                    }
                }

                model.LinkImage = imageLink;
                _userBusiness.CreateEmployee(model);

                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                //Xóa file trên server nếu cập nhật lỗi
                string[] mangErro = imageLink.Split(';');
                for (int i = 0; i < mangErro.Length; i++)
                {
                    UploadFileServer.DeleteFile(mangErro[i]);
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("GetEmployeeById")]
        [HttpPost]
        public HttpResponseMessage GetEmployeeById(string id)
        {
            try
            {
                var result = _userBusiness.GetEmployeeById(id);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [Route("UpdateEmployee")]
        [HttpPost]
        public HttpResponseMessage UpdateEmployee()
        {
            var modelJson = HttpContext.Current.Request.Form["Model"];
            EmployeeModel model = JsonConvert.DeserializeObject<EmployeeModel>(modelJson);
            HttpFileCollection hfc = HttpContext.Current.Request.Files;
            string pathFolder = "fileUpload/" + Constants.FolderEmployee + "/";
            string pathFolderServer = HostingEnvironment.MapPath("~/" + pathFolder);
            string ListFileUpdate = "";
            string imageLink = string.Empty;
            string imageLinkOld = string.Empty;
            try
            {
                if (hfc.Count > 0)
                {
                    for (int i = 0; i < hfc.Count; i++)
                    {
                        if (i == 0)
                        {
                            imageLink = Task.Run(async () =>
                            {
                                return await AzureStorageUploadFiles.GetInstance().UploadPhotoAsync(hfc[i], hfc[i].FileName, Constants.FolderEmployee);
                            }).Result;
                        }
                        else
                        {
                            imageLink += ";" + Task.Run(async () =>
                            {
                                return await AzureStorageUploadFiles.GetInstance().UploadPhotoAsync(hfc[i], hfc[i].FileName, Constants.FolderEmployee);
                            }).Result;
                        }
                    }
                }
                ListFileUpdate = model.LinkImage;
                if (!string.IsNullOrEmpty(imageLink))
                {
                    if (string.IsNullOrEmpty(model.LinkImage))
                    {
                        model.LinkImage = model.LinkImage + imageLink;
                    }
                    else
                    {
                        model.LinkImage = model.LinkImage + ";" + imageLink;
                    }
                }
                var link = _userBusiness.UpdateEmployee(model, imageLink);
                if (!string.IsNullOrEmpty(link))
                {
                    string[] linkmodel = ListFileUpdate.Split(';');
                    string[] mang = link.Split(';');
                    for (int i = 0; i < mang.Length; i++)
                    {
                        if (!linkmodel.Contains(mang[i]))
                        {
                            UploadFileServer.DeleteFile(mang[i]);
                        }
                    }
                }



                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                //Xóa file upload nếu cập nhật lỗi
                // UploadFileServer.DeleteFile(imageLink);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("DeleteEmployee")]
        [HttpPost]
        public HttpResponseMessage DeleteEmployee(string id, string CreateBy)
        {
            try
            {
                var img = _userBusiness.DeleteEmployee(id, CreateBy);
                if (!string.IsNullOrEmpty(img))
                {
                    string[] mang = img.Split(';');
                    for (int i = 0; i < mang.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(mang[i]))
                        {
                            UploadFileServer.DeleteFile(mang[i]);
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [Route("SearchTimeAttendanceLog")]
        [HttpPost]
        public HttpResponseMessage SearchTimeAttendanceLog(TimeAttendanceLogSearchCondition SearchCondition)
        {
            try
            {
                var result = _userBusiness.SearchTimeAttendanceLog(SearchCondition);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("SearchTimeAttendanceLogDetail")]
        [HttpPost]
        public HttpResponseMessage SearchTimeAttendanceLogDetail(TimeAttendanceLogSearchCondition SearchCondition)
        {
            try
            {
                #region[xu ly thoi gian]
                if (SearchCondition.Type.Equals("1"))
                {
                    SearchCondition.DateFrom = SearchCondition.Date;
                    SearchCondition.DateTo = SearchCondition.DateTo;
                }
                else if (SearchCondition.Type.Equals("2"))
                {
                    SearchCondition.DateFrom = SearchCondition.DateFrom;
                    SearchCondition.DateTo = SearchCondition.DateFrom.Value.AddDays(6);
               }
                else
                {
                    string thang = SearchCondition.Month < 10 ? "0" + SearchCondition.Month : SearchCondition.Month.ToString();
                    var dayMax = DateTime.DaysInMonth(SearchCondition.Year.Value, SearchCondition.Month.Value);
                    SearchCondition.DateFrom = DateTime.ParseExact(thang + "/01/" + SearchCondition.Year + " 00:00:01", "MM/dd/yyyy HH:mm:ss", null);
                    SearchCondition.DateTo = DateTime.ParseExact(thang + "/" + dayMax + "/" + SearchCondition.Year + " 23:59:59", "MM/dd/yyyy HH:mm:ss", null);
                }
                #endregion
                var result = _userBusiness.SearchTimeAttendanceLogDetail(SearchCondition);
                result.PathFile= System.Configuration.ConfigurationManager.AppSettings["UrlHostImage"]+ System.Configuration.ConfigurationManager.AppSettings["StorageContainer"]+"/";
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("TransactionLog")]
        [HttpPost]
        public HttpResponseMessage TransactionLog(TimeAttendanceLogSearchCondition searchCondition)
        {
            try
            {
                searchCondition.DateFrom = DateTime.Parse(searchCondition.DateFrom.Value.ToShortDateString() + " " + searchCondition.TimeFrom);
                searchCondition.DateTo = DateTime.Parse(searchCondition.DateTo.Value.ToShortDateString() + " " + searchCondition.TimeTo);
                var result = _userBusiness.TransactionLog(searchCondition);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex.InnerException);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("DensityLog")]
        [HttpPost]
        public HttpResponseMessage DensityLog(TimeAttendanceLogSearchCondition SearchCondition)
        {
            try
            {
                #region[xu ly thoi gian]

                //string ngayFrom = "";
                //string ngayTo = "";
                //if (SearchCondition.Week==1)
                //{
                //    ngayFrom = "01";
                //    ngayTo = "07";
                //}
                //else if (SearchCondition.Week==2)
                //{
                //    ngayFrom = "08";
                //    ngayTo = "14";
                //}
                //else if (SearchCondition.Week==3)
                //{
                //    ngayFrom = "15";
                //    ngayTo = "21";
                //}
                //else
                //{
                //    ngayFrom = "22";
                //    ngayTo = DateTime.DaysInMonth(SearchCondition.Year.Value, SearchCondition.Month.Value) + "";
                //}
                //string thang = SearchCondition.Month < 10 ? "0" + SearchCondition.Month : SearchCondition.Month.ToString();
                //SearchCondition.DateFrom = DateTime.ParseExact(thang + "/" + ngayFrom + "/" + SearchCondition.Year + " 00:00:01", "MM/dd/yyyy HH:mm:ss", null);
                //SearchCondition.DateTo = DateTime.ParseExact(thang + "/" + ngayTo + "/" + SearchCondition.Year + " 23:59:59", "MM/dd/yyyy HH:mm:ss", null);
                SearchCondition.DateFrom = SearchCondition.DateFrom;
                SearchCondition.DateTo = SearchCondition.DateFrom.Value.AddDays(6);
                #endregion
                var result = _userBusiness.DensityLog(SearchCondition);

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
