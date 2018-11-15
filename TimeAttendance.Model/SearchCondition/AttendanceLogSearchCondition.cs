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
using TimeAttendance.Model.SearchCondition;

namespace TimeAttendance.Model
{
    public class AttendanceLogSearchCondition: SearchConditionBase
    {
        public string CameraIPAddress { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string DepartmentId { get; set; }
        public string TimeTo { get; set; }
        public string TimeFrom { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public float ConfidenceFrom { get; set; }
        public float ConfidenceTo { get; set; }
        public int? FaceCount { get; set; }
    }
}
