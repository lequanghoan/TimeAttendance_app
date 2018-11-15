﻿using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TimeAttendance.API.Repositories;
using TimeAttendance.Model;

namespace TimeAttendance.API.Providers
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];
            var userid = context.Ticket.Properties.Dictionary["userid"];
            var securityKey = context.Ticket.Properties.Dictionary["securityKey"];

            if (string.IsNullOrEmpty(clientid) || string.IsNullOrEmpty(securityKey))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            using (AuthRepository _repo = new AuthRepository())
            {
                var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

                //var token = new RefreshToken()
                //{
                //    Id = Helper.GetHash(refreshTokenId),
                //    ClientId = clientid,
                //    Subject = context.Ticket.Identity.Name,
                //    IssuedUtc = DateTime.UtcNow,
                //    ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
                //};
                var token = new LoginCacheModel()
                {
                    userid= userid,
                    userName= context.Ticket.Identity.Name,
                    securityKey =securityKey,
                };
                //context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                //context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

                //token.ProtectedTicket = context.SerializeTicket();

                var result = await _repo.AddRefreshToken(token);

                if (result)
                {
                    context.SetToken(refreshTokenId);
                }
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = Helper.GetHash(context.Token);

            using (AuthRepository _repo = new AuthRepository())
            {
                var refreshToken = await _repo.FindRefreshToken(hashedTokenId);

                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = await _repo.RemoveRefreshToken(hashedTokenId);
                }
            }
        }
    }
}