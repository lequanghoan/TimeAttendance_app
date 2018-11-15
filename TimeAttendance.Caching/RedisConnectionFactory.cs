using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Utils;

namespace TimeAttendance.Caching
{
    public class RedisConnectionFactory
    {
        /// <summary>
        ///     The _connection.
        /// </summary>
        private readonly Lazy<ConnectionMultiplexer> _connection;


        public RedisConnectionFactory(string connectioString)
        {
            try
            {
                this._connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectioString));
            }
            catch (RedisTimeoutException redisEx)
            {
                throw new BusinessException(ErrorMessage.SYS001, redisEx);
            }
        }

        public ConnectionMultiplexer Connection()
        {
            return this._connection.Value;
        }
    }
}
