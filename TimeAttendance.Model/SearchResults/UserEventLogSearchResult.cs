using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model.SearchResults
{
    public class UserEventLogSearchResult
    {
        /// <summary> 
        /// UserEventLogId 
        /// </summary> 
        public string UserEventLogId { get; set; }
        public string ObjectId { get; set; }
        /// <summary> 
        /// Id nguười dùng
        /// </summary> 
        public string UserId { get; set; }
        /// <summary> 
        /// Mô tả
        /// </summary> 
        public string Description { get; set; }
        /// <summary> 
        /// Loại
        /// </summary> 
        public Nullable<int> LogType { get; set; }
        /// <summary> 
        /// LogGroup 
        /// </summary> 
        public Nullable<int> LogGroup { get; set; }

        public string Type { get; set; }

        /// <summary> 
        /// CreateDate 
        /// </summary> 
        public Nullable<DateTime> CreateDate { get; set; }
        public string LogTypeName { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; }
    }
}
