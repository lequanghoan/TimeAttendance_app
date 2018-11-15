using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class SearchResultObject <T>
    {
        public int TotalItem { get; set; }
        public int TotalItemOkCount { get; set; }
        public int TotalItemOk { get; set; }
        public int TotalItemNotOk { get; set; }

        public List<T> ListResult = new List<T>();

        public string PathFile { get; set; }
        public List<string> listDay = new List<string>();
        public List<string> listTime = new List<string>();
    }
}
