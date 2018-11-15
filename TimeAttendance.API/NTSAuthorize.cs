using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Script.Serialization;
using TimeAttendance.API.Repositories;

namespace TimeAttendance.API
{
    public class NTSAuthorize : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        public string AllowFeature { get; set; }
        private string userName = string.Empty;
        private string securityKey = string.Empty;
        
        private string authorizeString = string.Empty;
        private string authorizeItemString = string.Empty;
        private ClaimsPrincipal principal;
        private string[] allowFeatureList;
        private bool isAuthorize;
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var filterAttribute = actionContext.ActionDescriptor.GetCustomAttributes<NTSAuthorize>(true)
                .Where(a => a.GetType() == typeof(NTSAuthorize));

            if (filterAttribute != null)
            {
                foreach (NTSAuthorize attr in filterAttribute)
                {
                    AllowFeature = attr.AllowFeature;
                }
                principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

                try
                {
                    userName = principal.Claims.Where(c => c.Type == "user").Single().Value;
                    securityKey = principal.Claims.Where(c => c.Type == "securityKey").Single().Value;
                    bool isTokenAlive = false;
                    using (AuthRepository _repo = new AuthRepository())
                    {
                        isTokenAlive = _repo.IsTokenAlive(userName, securityKey);
                    }

                    if (!isTokenAlive)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Bạn đã hết phiên làm việc. Bạn hãy đăng nhập lại để tiếp tục.");
                    }
                    else {
                        authorizeString = principal.Claims.Where(c => c.Type == "AuthorizeString").Single().Value;

                        //if not have permission
                        if (AllowFeature == null || !CheckRole(AllowFeature, authorizeString))
                        {
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.MethodNotAllowed, "Bạn không có quyền thao tác dữ liệu này.");
                        }
                    }
                }
                catch
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Bạn đã hết phiên làm việc. Bạn hãy đăng nhập lại để tiếp tục.");
                }
            }
            base.OnAuthorization(actionContext);
        }

        private bool CheckRole(string allowFeature, string authorize)
        {
            isAuthorize = false;
            allowFeatureList = allowFeature.Split(';');
            var jss = new JavaScriptSerializer();
            List<string> listPermission = jss.Deserialize<List<string>>(authorize).ToList();
            if (listPermission != null && listPermission.Count() > 0)
            {
                foreach (var item in allowFeatureList)
                {
                    if (listPermission.Any(a => a.Equals(item.Trim())))
                    {
                        isAuthorize = true;
                    }
                }
            }

            return isAuthorize;
        }
    }
}