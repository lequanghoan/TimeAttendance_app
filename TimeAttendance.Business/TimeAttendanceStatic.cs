using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Model;

namespace TimeAttendance.Business
{
    public static class TimeAttendanceStatic
    {
        /// <summary>
        /// Danh sách nhân viên
        /// </summary>
        public static List<InfoEmployeeModel> ListInfoEmployee = null;
        /// <summary>
        /// Giờ bắt đầu
        /// </summary>
        public static string TimeIn = string.Empty;
        /// <summary>
        /// Giờ kết thúc
        /// </summary>
        public static string TimeOut = string.Empty;
        /// <summary>
        /// thoi gian ghi log
        /// </summary>
        public static string TimeAttendanceLog = string.Empty;
        /// <summary>
        /// Giờ bắt đầu
        /// </summary>
        public static Nullable<DateTime> StartTime = null;
        /// <summary>
        /// Giờ kết đầu
        /// </summary>
        public static Nullable<DateTime> EndTime = null;
        /// <summary>
        /// Ngày hiện tại
        /// </summary>
        public static Nullable<DateTime> DateNow = null;

        public static double ConfidenceFix = 0;

        public static Nullable<DateTime> ProcessTime = null;
        /// <summary>
        /// Thời gian bắt đầu không nhận chấm công
        /// </summary>
        public static string NotProcessTime = string.Empty;
    }
}
