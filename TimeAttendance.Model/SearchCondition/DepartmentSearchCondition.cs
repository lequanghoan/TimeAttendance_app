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
    public class DepartmentSearchCondition: SearchConditionBase
    {
        public string Name { get; set; }
    }
}
