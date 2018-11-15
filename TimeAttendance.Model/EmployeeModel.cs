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
    public class EmployeeModel
    {
        public bool EmpSelect { get; set; }
        public string EmployeeId { get; set; }
        public string DepartmentId { get; set; }
        public string JobTitleId { get; set; }
        public string FaceId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public DateTime? InComeDate { get; set; }
        public DateTime? OutComeDate { get; set; }
        public string Address { get; set; }
        public string IdentifyCardNumber { get; set; }
        public string Description { get; set; }
        public string LinkImage { get; set; }
        public string LinkImageFaceId { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
