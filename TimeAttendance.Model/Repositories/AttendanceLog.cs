//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimeAttendance.Model.Repositories
{
    using System;
    using System.Collections.Generic;
    
    public partial class AttendanceLog
    {
        public string AttendanceLogId { get; set; }
        public string EmployeeId { get; set; }
        public string ClientIPAddress { get; set; }
        public string CameraIPAdress { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string ImageLink { get; set; }
        public Nullable<double> Confidence { get; set; }
        public Nullable<int> FaceCount { get; set; }
        public string ImageFace { get; set; }
        public string Note { get; set; }
    }
}
