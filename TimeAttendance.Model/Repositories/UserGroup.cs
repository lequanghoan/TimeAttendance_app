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
    
    public partial class UserGroup
    {
        public string UserGroupId { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }
    
        public virtual Group Group { get; set; }
        public virtual Group Group1 { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
