using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace TimeAttendance.Model
{
    public class NoAttendanceLogSearchResult
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitleId { get; set; }
        public string JobTitleName { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
