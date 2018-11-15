using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model.CacheModel
{
  public  class DensityCache
    {
        public List<MCRP3100HeatMap> ListResult { get; set; }
        public List<string> listDay { get; set; }
        public List<string> listTime { get; set; }
        public string DateFrom_ToKey { get; set; }
        public DensityCache()
        {
            ListResult = new List<MCRP3100HeatMap>();
            listDay = new List<string>();
            listTime = new List<string>();
        }
    }
}
