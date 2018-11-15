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
    public class EmployeeSearchCondition: SearchConditionBase
    {
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? Gender { get; set; }
        public string Address { get; set; }
        public string IdentifyCardNumber { get; set; }

    }
}
