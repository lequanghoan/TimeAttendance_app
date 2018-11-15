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
    public class ConfigResult
    {
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public int Percent { get; set; }
        public int TimeAttendanceLog { get; set; }

    }
}
