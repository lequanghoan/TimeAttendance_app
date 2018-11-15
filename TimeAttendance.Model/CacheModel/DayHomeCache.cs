using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model.CacheModel
{
  public  class DayHomeCache
    {
        public List<DataResultModel> ListDataDay { get; set; }
        public double LatePercent { get; set; }
        public double EarlyPercent { get; set; }
        public double AbsentPercent { get; set; }
        public string DateNowKey { get; set; }

        public DayHomeCache()
        {
            ListDataDay = new List<DataResultModel>();
        }
    }
}
