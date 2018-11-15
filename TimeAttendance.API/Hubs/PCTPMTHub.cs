using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeAttendance.API
{
    public class TimeAttendanceHub : Hub
    {
        public void NotifyDocument()
        {
            Clients.All.NotifyDocument();
        }
    }
}