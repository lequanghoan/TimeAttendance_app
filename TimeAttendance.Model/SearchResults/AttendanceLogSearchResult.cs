using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace TimeAttendance.Model
{
    public class AttendanceLogSearchResult
    {
        public string AttendanceLogId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeOldId { get; set; }

        public string ClientIPAddress { get; set; }
        public string CameraIPAddress { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string ImageLink { get; set; }
        public string ImageFace { get; set; }
        public double? Confidence { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeCodeName { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleName { get; set; }
        public string JobTitleId { get; set; }
        public string DepartmentName { get; set; }
        public string TimeTo { get; set; }
        public string TimeFrom { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? FaceCount { get; set; }
        public bool ObjSelect { get; set; }
    }

}
