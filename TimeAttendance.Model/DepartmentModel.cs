using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace TimeAttendance.Model
{
    public class DepartmentModel
    {
        public string DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }

}
