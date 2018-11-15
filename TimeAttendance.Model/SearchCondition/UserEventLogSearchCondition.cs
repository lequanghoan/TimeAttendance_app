using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model.SearchCondition
{
    public class UserEventLogSearchCondition : SearchConditionBase
    {
        /// <summary> 
        /// Mô tả
        /// </summary> 
        public string Description { get; set; }
        /// <summary> 
        /// Loại
        /// </summary> 
        public Nullable<int> LogType { get; set; }

        public string UserType { get; set; }

        public Nullable<DateTime> LogDateFrom { get; set; }

        public Nullable<DateTime> LogDateTo { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        /// <summary> 
        /// Id nguười dùng tìm kiếm
        /// </summary> 
        public string UserIdSearch { get; set; }
    }
}
