// <copyright company="nhantinsoft.vn">
// Author: Vũ Văn Văn
// Created Date: 30/09/2017 13:08
// </copyright>
using TimeAttendance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class TransactionLogSearchResult
    {
        public string TransactionLogId { get; set; }
        public string ClientIPAddress { get; set; }
        public string CameraIPAdress { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? CallDateTime { get; set; }
        public DateTime? ResponseDateTime { get; set; }
        public string ImageLink { get; set; }
        public string StatusCode { get; set; }
        public string StatusCodeView { get; set; }
        public double? ResponseTime { get; set; }

    }

    public class MCRP3100HeatMap
    {
        public int x { get; set; }
        public int y { get; set; }
        public int VehicleInStatistic { get; set; }
        public int VehicleOutStatistic { get; set; }
        public string DateKey { get; set; }
    }
}
