using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model.SearchCondition
{
    public class GroupUserSearchCondition: SearchConditionBase
    {
        /// <summary> 
        /// Tên nhóm 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        /// Trạng thái 
        /// </summary> 
        public Nullable<int> Status { get; set; }
        /// <summary> 
        /// Mô tả 
        /// </summary> 
        public string Description { get; set; }
    }
}
