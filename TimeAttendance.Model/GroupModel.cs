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
    public class GroupModel
    {
        /// <summary> 
        /// Id 
        /// </summary> 
        public string GroupId { get; set; }
        /// <summary> 
        /// Tên nhóm 
        /// </summary> 
        public string Name { get; set; }
        /// <summary> 
        /// Trang làm việc sau khi login
        /// </summary> 
        public string HomePage { get; set; }

        /// <summary> 
        /// Trạng thái 
        /// </summary> 
        public Nullable<int> Status { get; set; }
        /// <summary> 
        /// Mô tả 
        /// </summary> 
        public string Description { get; set; }
        /// <summary> 
        /// CreateBy 
        /// </summary> 
        public string CreateBy { get; set; }
        /// <summary> 
        /// CreateDate 
        /// </summary> 
        public Nullable<DateTime> CreateDate { get; set; }
        /// <summary> 
        /// UpdateBy 
        /// </summary> 
        public string UpdateBy { get; set; }
        /// <summary> 
        /// UpdateDate 
        /// </summary> 
        public Nullable<DateTime> UpdateDate { get; set; }

        /// <summary>
        /// Dánh sách quyền trong nhóm
        /// </summary>
        public List<FunctionModel> ListPermission { get; set; }

        public string UserId { get; set; }
    }
}
