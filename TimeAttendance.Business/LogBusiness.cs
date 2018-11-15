using TimeAttendance.Utils;
using TimeAttendance.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;

namespace TimeAttendance.Business
{
    public class LogBusiness
    {
      private  FaceServiceClient faceClient { get; set; }
        /// <summary>
        /// Log đang nhập
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userName"></param>
        public static void SaveLogLogin(TimeAttendanceEntities db, string userId)
        {
            int logType = Constants.LogTypeDataMining;
            UserEventLog model = new UserEventLog()
            {
                UserEventLogId = Guid.NewGuid().ToString(),
                UserId = userId,
                Description = "Đăng nhập hệ thống",
                LogType = logType,
                CreateDate = DateTime.Now,
            };
            db.UserEventLog.Add(model);
            db.SaveChanges();
        }

        public static void SaveLogLogout(TimeAttendanceEntities db, string userId)
        {
            int logType = Constants.LogTypeAccess;
            UserEventLog model = new UserEventLog()
            {
                UserEventLogId = Guid.NewGuid().ToString(),
                UserId = userId,
                Description = "Đăng xuất khỏi hệ thống",
                LogType = logType,
                CreateDate = DateTime.Now,
            };
            db.UserEventLog.Add(model);
            db.SaveChanges();
        }

        /// <summary>
        /// Log thao tác dữ liệu
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userName"></param>
        /// <param name="content"></param>
        /// <param name="objectId"></param>
        public static void SaveLogEvent(TimeAttendanceEntities db, string userId, string description, string objectId)
        {
            int logType = Constants.LogTypeDataMining;
            UserEventLog model = new UserEventLog()
            {
                UserEventLogId = Guid.NewGuid().ToString(),
                UserId = userId,
                Description = description,
                LogType = logType,
                CreateDate = DateTime.Now
            };
            db.UserEventLog.Add(model);
            //db.SaveChanges();
        }
    }
}
