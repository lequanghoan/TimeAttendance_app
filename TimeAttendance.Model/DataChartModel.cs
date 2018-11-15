using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class DataResultModel
    {
        public string Name { get; set; }
        public int TotalLate { get; set; }
        public int TotalEarly { get; set; }
        public int TotalAbsent { get; set; }
    }

    public class DataChartModel
    {
        public List<string> ListCategories { get; set; }
        public List<int> ListTotalLate { get; set; }
        public List<int> ListTotalEarly { get; set; }
        public List<int> ListTotalAbsent { get; set; }
        public DataChartModel()
        {
            ListCategories = new List<string>();
            ListTotalLate = new List<int>();
            ListTotalEarly = new List<int>();
            ListTotalAbsent = new List<int>();
        }
    }

    public class DataTopModel
    {
        public string EmployeeName { get; set; }
        public string Code { get; set; }
        public int Total { get; set; }
        public int TotalTime { get; set; }
    }
}
