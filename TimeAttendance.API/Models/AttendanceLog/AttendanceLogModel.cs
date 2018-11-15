using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace TimeAttendance.API.Model
{
    public class AttendanceLogModel
    {
        public string AttendanceLogId { get; set; }
        public string EmployeeId { get; set; }
        public string ClientIPAddress { get; set; }
        public string CameraIPAddress { get; set; }
        public DateTime Date { get; set; }
        public string ImageLink { get; set; }
        public decimal Confidence { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string DepartmentName { get; set; }
        public string TimeTo { get; set; }
        public string TimeFrom { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}
