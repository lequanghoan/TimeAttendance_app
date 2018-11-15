using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Model.Repositories;

namespace TimeAttendance.Model
{
    public class DashboardResultModel
    {
        /// <summary>
        /// Tổng số nhân viên
        /// </summary>
        public int TotalEmployee { get; set; }

        /// <summary>
        /// Ngày hiện tại
        /// </summary>
        public DateTime DateNow { get; set; }
        public DateTime DateBefore { get; set; }

        public string DayNow { get; set; }

        /// <summary>
        /// Tuần hiện tại
        /// </summary>
        public int WeekNow { get; set; }
        /// <summary>
        /// Tuần trước
        /// </summary>
        public int WeekBefore { get; set; }

        /// <summary>
        /// Tháng hiện tại
        /// </summary>
        public int MonthNow { get; set; }
        /// <summary>
        /// Tháng trước
        /// </summary>
        public int MonthBefore { get; set; }

        /// <summary>
        /// Quý hiện tại
        /// </summary>
        public int QuarterNow { get; set; }
        /// <summary>
        /// Quý trước
        /// </summary>
        public int QuarterBefore { get; set; }

        //Data ngày hiện tại
        public List<DataResultModel> ListDataDay { get; set; }
        public double LatePercent { get; set; }
        public double EarlyPercent { get; set; }
        public double AbsentPercent { get; set; }

        //Data tuần
        public List<DataResultModel> ListDataWeek { get; set; }
        public DataChartModel DataChartWeek { get; set; }
        public List<DataTopModel> ListLateTopInWeek { get; set; }
        public List<DataTopModel> ListEarlyTopInWeek { get; set; }

        //Data tuần
        public List<DataResultModel> ListDataMonth { get; set; }
        public DataChartModel DataChartMonth { get; set; }
        public List<DataTopModel> ListLateTopInMonth { get; set; }
        public List<DataTopModel> ListEarlyTopInMonth { get; set; }

        //Data quý
        public List<DataResultModel> ListDataQuarter { get; set; }
        public DataChartModel DataChartQuarter { get; set; }
        public List<DataTopModel> ListLateTopInQuarter { get; set; }
        public List<DataTopModel> ListEarlyTopInQuarter { get; set; }
    }
}
