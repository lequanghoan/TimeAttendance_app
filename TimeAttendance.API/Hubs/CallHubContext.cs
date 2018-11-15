using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeAttendance.API
{
    public class CallHubContext
    {
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<TimeAttendanceHub>();

        public static void HubNotifyDocument()
        {
            hubContext.Clients.All.NotifyDocument();
        }
    }
}