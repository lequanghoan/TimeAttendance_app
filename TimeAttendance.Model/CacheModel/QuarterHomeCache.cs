using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model.CacheModel
{
  public  class QuarterHomeCache
    {
        public List<DataResultModel> ListDataQuarter { get; set; }
        public List<DataTopModel> ListLateTopInQuarter { get; set; }
        public List<DataTopModel> ListEarlyTopInQuarter { get; set; }
        public DataChartModel DataChartQuarter { get; set; }
        public string DateFrom_ToKey { get; set; }
        public QuarterHomeCache()
        {
            ListDataQuarter = new List<DataResultModel>();
            ListLateTopInQuarter = new List<DataTopModel>();
            ListEarlyTopInQuarter = new List<DataTopModel>();
            DataChartQuarter = new DataChartModel();
        }
    }
}
