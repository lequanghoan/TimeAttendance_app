using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Utils;

namespace TimeAttendance.Caching
{
    public class RedisService<T>
    {
        internal readonly IDatabase Database;
        protected readonly RedisConnectionFactory ConnectionFactory;
        private static string redisCacheConnection = ConfigurationManager.AppSettings["RedisConnection"];

        private RedisService()
        {
            this.ConnectionFactory = new RedisConnectionFactory(redisCacheConnection);
            this.Database = this.ConnectionFactory.Connection().GetDatabase();
        }

        private static RedisService<T> singletonObject;

        public static RedisService<T> GetInstance(string RedisConnection)
        {
            if (string.IsNullOrEmpty(redisCacheConnection))
            {
                redisCacheConnection = RedisConnection;
            }
            if (singletonObject == null)
            {
                singletonObject = new RedisService<T>();
            }

            return singletonObject;
        }

        public List<T> GetAll(string key)
        {
            List<T> model = new List<T>();
            try
            {
                string serialized = Database.StringGet(key);
                if (!string.IsNullOrEmpty(serialized))
                {
                    model = JsonConvert.DeserializeObject<List<T>>(serialized);
                }

                return model;
            }
            catch (TimeoutException ex)
            {
                return model;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessage.SYS001, ex);
            }
        }
        public void SetAll(string key, List<T> obj)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException();
                }

                if (obj != null)
                {
                    // Set expired time = 15*24h
                    Database.StringSet(key, JsonConvert.SerializeObject(obj), new TimeSpan(360, 0, 0));
                }
            }
            catch (TimeoutException ex)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessage.SYS001, ex);
            }
        }

        public bool Exists(string key)
        {
            return this.Database.KeyExists(key, CommandFlags.None);
        }
        public Task<bool> ExistsAsync(string key)
        {
            return this.Database.KeyExistsAsync(key, CommandFlags.None);
        }
        public bool Remove(string key)
        {
            return this.Database.KeyDelete(key, CommandFlags.None);
        }
        public Task<bool> RemoveAsync(string key)
        {
            return this.Database.KeyDeleteAsync(key, CommandFlags.None);
        }

        public T Get<T>(string key)
        {
            RedisValue value = this.Database.StringGet(key, CommandFlags.None);
            if (!value.HasValue)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(value);
        }
        public async Task<T> GetAsync<T>(string key)
        {
            RedisValue value = await this.Database.StringGetAsync(key, CommandFlags.None);
            T result;
            if (!value.HasValue)
            {
                result = default(T);
            }
            else
            {
                result = JsonConvert.DeserializeObject<T>(value);
            }
            return result;
        }
        public bool Add<T>(string key, T value)
        {
            string value2 = JsonConvert.SerializeObject(value);
            return this.Database.StringSet(key, value2, null, When.Always, CommandFlags.None);
        }

        public bool Add<T>(string key, T value, DateTimeOffset expiresAt)
        {
            string value2 = JsonConvert.SerializeObject(value);
            TimeSpan expireTime = expiresAt.Subtract(DateTimeOffset.Now);
            return this.Database.StringSet(key, value2, new TimeSpan?(expireTime), When.Always, CommandFlags.None);
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            string value2 = JsonConvert.SerializeObject(value);
            return this.Database.StringSet(key, value2, new TimeSpan?(expiresIn), When.Always, CommandFlags.None);
        }

        public bool Replace<T>(string key, T value)
        {
            return this.Add<T>(key, value);
        }

        public bool Replace<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return this.Add<T>(key, value, expiresAt);
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return this.Add<T>(key, value, expiresIn);
        }

        public bool AddAll<T>(IList<Tuple<string, T>> items)
        {
            KeyValuePair<RedisKey, RedisValue>[] values = (
                from item in items
                select new KeyValuePair<RedisKey, RedisValue>(item.Item1, JsonConvert.SerializeObject(item.Item2))).ToArray<KeyValuePair<RedisKey, RedisValue>>();
            return this.Database.StringSet(values, When.Always, CommandFlags.None);
        }
        public async Task<bool> AddAllAsync<T>(IList<Tuple<string, T>> items)
        {
            KeyValuePair<RedisKey, RedisValue>[] values = (
                from item in items
                select new KeyValuePair<RedisKey, RedisValue>(item.Item1, JsonConvert.SerializeObject(item.Item2))).ToArray<KeyValuePair<RedisKey, RedisValue>>();
            return await this.Database.StringSetAsync(values, When.Always, CommandFlags.None);
        }

        public bool SetAdd<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null.");
            }
            string value = JsonConvert.SerializeObject(item);
            return this.Database.SetAdd(key, value, CommandFlags.None);
        }

        public bool SetRemove<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null.");
            }
            string value = JsonConvert.SerializeObject(item);
            return this.Database.SetRemove(key, value, CommandFlags.None);
        }

        public void FlushDb()
        {
            EndPoint[] endPoints = this.Database.Multiplexer.GetEndPoints(false);
            for (int i = 0; i < endPoints.Length; i++)
            {
                EndPoint endpoint = endPoints[i];
                this.Database.Multiplexer.GetServer(endpoint, null).FlushDatabase(this.Database.Database, CommandFlags.None);
            }
        }
        public async Task FlushDbAsync()
        {
            EndPoint[] endPoints = this.Database.Multiplexer.GetEndPoints(false);
            EndPoint[] array = endPoints;
            for (int i = 0; i < array.Length; i++)
            {
                EndPoint endpoint = array[i];
                await this.Database.Multiplexer.GetServer(endpoint, null).FlushDatabaseAsync(this.Database.Database, CommandFlags.None);
            }
            array = null;
        }
        //public void Save(SaveType saveType)
        //{
        //    EndPoint[] endPoints = this.Database.Multiplexer.GetEndPoints(false);
        //    for (int i = 0; i < endPoints.Length; i++)
        //    {
        //        EndPoint endpoint = endPoints[i];
        //        this.Database.Multiplexer.GetServer(endpoint, null).Save(saveType, CommandFlags.None);
        //    }
        //}
        //public async void SaveAsync(SaveType saveType)
        //{
        //    EndPoint[] endPoints = this.Database.Multiplexer.GetEndPoints(false);
        //    EndPoint[] array = endPoints;
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        EndPoint endpoint = array[i];
        //        await this.Database.Multiplexer.GetServer(endpoint, null).SaveAsync(saveType, CommandFlags.None);
        //    }
        //    array = null;
        //}
    }
}
