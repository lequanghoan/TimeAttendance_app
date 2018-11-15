using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TimeAttendance.API;
using TimeAttendance.Model;
using TimeAttendance.Business;
using TimeAttendance.Caching;
using Newtonsoft.Json;
namespace TimeAttendance.API.Repositories
{
    /// <summary>
    /// We are depending on the “UserManager” that provides the domain logic for working with user information.
    /// The “UserManager” knows when to hash a password, how and when to validate a user, and how to manage claims
    /// </summary>
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        private UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.Name
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<LoginEntity> FindUser(string userName, string password)
        {
            LoginEntity loginBO = new UserBusiness().Login(userName, password);

            return loginBO;
        }

        #region Token refreshing
        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);

            return client;
        }


        public async Task<bool> AddRefreshToken(LoginCacheModel token)
        {
            try
            {
                var RedisConnection = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"];
                var redis = RedisService<LoginCacheModel>.GetInstance(RedisConnection);
                LoginCacheModel RefreshToken;
                RefreshToken = redis.Get<LoginCacheModel>("AT:Login:" + token.userName);
                if (RefreshToken != null)
                {
                    if (RefreshToken.userName.Equals(token.userName) && RefreshToken.securityKey.Equals(token.securityKey))
                    {
                        redis.Remove("AT:Login:" + token.userName);
                    }
                }
                redis.Add("AT:Login:" + token.userName, token);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> AddRefreshTokenOLd(RefreshToken token)
        {
            var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).ToList();

            if (existingToken != null && existingToken.Count() > 0)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            _ctx.RefreshTokens.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public bool IsTokenAlive(string accountName,string securityKey)
        {
            try
            {
                var RedisConnection = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"];

                var redis = RedisService<LoginCacheModel>.GetInstance(RedisConnection);
                LoginCacheModel RefreshToken;
                RefreshToken = redis.Get<LoginCacheModel>("AT:Login:" + accountName);
                if (RefreshToken != null)
                {
                    if (RefreshToken.userName.Equals(accountName) && RefreshToken.securityKey.Equals(securityKey))
                    { return true; }
                    else
                    { return false; }
                }
                else
                { return false; }
            }
            catch (Exception)
            { return false; }
        }
        public bool IsTokenAliveOld(string accountName)
        {
            var existingToken = _ctx.RefreshTokens.Where(r => r.Subject.Equals(accountName));

            return existingToken.Count() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(List<RefreshToken> refreshToken)
        {
            foreach (var itemRefreshToken in refreshToken)
                _ctx.RefreshTokens.Remove(itemRefreshToken);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }
        #endregion Token refreshing
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}