using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Business;
using TimeAttendance.Model.Entities;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            FaceHelperFuntionBusiness _buss;

            // var RedisConnection = "103.78.88.118:6379,abortConnect=false,ssl=false,password=nhantinsoft123456!";
            var RedisConnection = "timeattendance-poc.redis.cache.windows.net:6380,password=trfVpGgQKVs1Xm1hJHUzoPSgUeUN54P8LwDC9jxsFaE=,ssl=True,abortConnect=False";
            var connStr = "metadata=res://*/Repositories.TimeAttendanceModel.csdl|res://*/Repositories.TimeAttendanceModel.ssdl|res://*/Repositories.TimeAttendanceModel.msl;provider=System.Data.SqlClient;provider connection string=\";data source=tcp:attendtime.database.windows.net,1433;Initial Catalog=TimeAttendance;Persist Security Info=False;User ID=nts;Password=nhantinsoft123456!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;\";";
            ConnectionModel connectionModel = new ConnectionModel();
            connectionModel.RedisConnection = RedisConnection;
            connectionModel.connStr = connStr;
            _buss = new FaceHelperFuntionBusiness(connectionModel);

            string str = "Data Statistics: Calculate Attendance Time";
            try
            {
                _buss.AddOrUpdateCacheColectionFuntion();
            }
            catch (Exception exception)
            {
            }
        }
    }
}
