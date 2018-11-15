using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model.SearchResults
{
    public class GroupSearchResult
    {
        public string GroupId { get; set; }
        public string Name { get; set; }
        public string HomePage { get; set; }
        public int? Status { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int CountUser { get; set; }
      
    }
}
