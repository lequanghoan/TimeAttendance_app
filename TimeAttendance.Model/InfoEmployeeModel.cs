using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class InfoEmployeeModel
    {
        public string EmployeeId { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitleName { get; set; }
        public string FaceId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Nullable<DateTime> DateOfBirth { get; set; }
        public string GenderName { get; set; }
        public Nullable<DateTime> InComeDate { get; set; }
        public Nullable<DateTime> OutComeDate { get; set; }
        public string IdentifyCardNumber { get; set; }
        public string LinkImage { get; set; }
    }
}
