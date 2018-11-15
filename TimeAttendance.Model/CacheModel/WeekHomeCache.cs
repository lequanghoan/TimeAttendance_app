using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model.CacheModel
{
  public  class WeekHomeCache
    {
        public List<DataResultModel> ListDataWeek { get; set; }
        public List<DataTopModel> ListLateTopInWeek { get; set; }
        public List<DataTopModel> ListEarlyTopInWeek { get; set; }
        public DataChartModel DataChartWeek { get; set; }
        public string DateFrom_ToKey { get; set; }
        public WeekHomeCache()
        {
            ListDataWeek = new List<DataResultModel>();
            ListLateTopInWeek = new List<DataTopModel>();
            ListEarlyTopInWeek = new List<DataTopModel>();
            DataChartWeek = new DataChartModel();
        }
    }
}
