using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model.CacheModel
{
  public  class MonthHomeCache
    {
        public List<DataResultModel> ListDataMonth { get; set; }
        public List<DataTopModel> ListLateTopInMonth { get; set; }
        public List<DataTopModel> ListEarlyTopInMonth { get; set; }
        public DataChartModel DataChartMonth { get; set; }
        public string DateFrom_ToKey { get; set; }
        public MonthHomeCache()
        {
            ListDataMonth = new List<DataResultModel>();
            ListLateTopInMonth = new List<DataTopModel>();
            ListEarlyTopInMonth = new List<DataTopModel>();
            DataChartMonth = new DataChartModel();
        }
    }
}
