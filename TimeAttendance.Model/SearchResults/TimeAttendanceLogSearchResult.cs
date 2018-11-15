// <copyright company="nhantinsoft.vn">
// Author: Vũ Văn Văn
// Created Date: 30/09/2017 13:08
// </copyright>
using TimeAttendance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class TimeAttendanceLogSearchResult
    {
        public string TimeAttendanceLogId { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitleName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public string ImageIn { get; set; }
        public string ImageOut { get; set; }
        public int? Total { get; set; }
        public int? LateMinutes { get; set; }
        public int? EarlyMinutes { get; set; }
        public string DayView { get; set; }
        public string ImageFaceIn { get; set; }
        public string ImageFaceOut { get; set; }

    }
}
