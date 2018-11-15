using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class AttendanceLogSearchResultObject 
    {
        public int TotalItem { get; set; }
        public string PathExport { get; set; }
        public string PathFile { get; set; }
        public List<AttendanceLogSearchResult> ListResult = new List<AttendanceLogSearchResult>();

    }
}
