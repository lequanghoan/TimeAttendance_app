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
    public class TimeAttendanceLogSearchCondition : SearchConditionBase
    {

        public int Export { get; set; }
     
        public string Type { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Week { get; set; }
        public DateTime? Date { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string StatusCode { get; set; }
      public string TimeFrom { get; set; }
      public string TimeTo { get; set; }
        public string ClientIPAddress { get; set; }
        public string CameraIPAdress { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
    }
}
