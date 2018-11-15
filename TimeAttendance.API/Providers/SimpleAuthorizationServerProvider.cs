using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using TimeAttendance.API.Repositories;
using TimeAttendance.API;
using Newtonsoft.Json;
using TimeAttendance.Model;

namespace TimeAttendance.API.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// Called to validate that the origin of the request is a registered "client_id",
        //     and that the correct credentials for that client are present on the request.
        //     If the web application accepts Basic authentication credentials, context.TryGetBasicCredentials(out
        //     clientId, out clientSecret) may be called to acquire those values if present
        //     in the request header. If the web application accepts "client_id" and "client_secret"
        //     as form encoded POST parameters, context.TryGetFormCredentials(out clientId,
        //     out clientSecret) may be called to acquire those values if present in the
        //     request body.  If context.Validated is not called the request will not proceed
        //     further.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            Client client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            using (AuthRepository _repo = new AuthRepository())
            {
                client = _repo.FindClient(context.ClientId);
            }

            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null) allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type, Accept" });


            LoginEntity loginBO;
            using (AuthRepository _repo = new AuthRepository())
            {
                loginBO = await _repo.FindUser(context.UserName, context.Password);

                if (loginBO.ResponseCode != 0)
                {
                    context.SetError("invalid_grant", loginBO.ResponseMessage);
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("user", context.UserName));
            identity.AddClaim(new Claim("securityKey", loginBO.UserInfor.securityKey));

            var listStrPermission = loginBO.UserInfor.ListPermission != null ? JsonConvert.SerializeObject(loginBO.UserInfor.ListPermission) : string.Empty;
            identity.AddClaim(new Claim("AuthorizeString", listStrPermission));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId},
                    { "userName", context.UserName== null? string.Empty : context.UserName},
                    { "homePage", loginBO.UserInfor.HomePage== null? string.Empty : loginBO.UserInfor.HomePage},
                    { "userfullname", loginBO.UserInfor.FullName == null? string.Empty : loginBO.UserInfor.FullName},
                    { "userid", loginBO.UserInfor.UserId== null? string.Empty : loginBO.UserInfor.UserId},
                    { "permission", listStrPermission== null? string.Empty : listStrPermission},
                   // { "id", loginBO.UserInfor.Id== null? string.Empty : loginBO.UserInfor.Id},
                  //  { "code", loginBO.UserInfor.Code == null? string.Empty : loginBO.UserInfor.Code},
                    { "imageLink", loginBO.UserInfor.ImageLink == null? string.Empty: loginBO.UserInfor.ImageLink},
                   // { "unitid", loginBO.UserInfor.UnitId == null? string.Empty : loginBO.UserInfor.UnitId},
                   // { "level",  loginBO.UserInfor.Type.ToString()},
                    //{ "IsAdmin",  loginBO.UserInfor.IsAdmin.ToString()},
                      { "securityKey", loginBO.UserInfor.securityKey == null? string.Empty : loginBO.UserInfor.securityKey},
                    { "groupid", loginBO.UserInfor.GroupId == null? string.Empty : loginBO.UserInfor.GroupId},
                   // { "policeLevel", loginBO.UserInfor.GroupId == null? string.Empty : loginBO.UserInfor.PoliceLevel},
                 //   { "agency", loginBO.UserInfor.Agency == null? string.Empty : loginBO.UserInfor.Agency},
                  
                    { "type", loginBO.UserInfor.Type == null? string.Empty : loginBO.UserInfor.Type},
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }
    }
}