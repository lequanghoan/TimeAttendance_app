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
    public class UserModel
    {
        /// <summary> 
        /// id 
        /// </summary> 
        public string UserId { get; set; }
        /// <summary> 
        /// Chức vụ 
        /// </summary> 
        public string UserRoleId { get; set; }
        /// <summary> 
        /// Nhóm người dùng
        /// </summary> 
        public string GroupId { get; set; }

        /// <summary> 
        /// Tên đăng nhập 
        /// </summary> 
        public string Name { get; set; }
        /// <summary> 
        /// Họ tên 
        /// </summary> 
        public string FullName { get; set; }
        /// <summary> 
        /// Ngày sinh 
        /// </summary> 
        public Nullable<DateTime> BirthDay { get; set; }
        /// <summary>
        /// Cơ quan
        /// </summary>
        public string Agency { get; set; }
        /// <summary> 
        /// Email 
        /// </summary> 
        public string Email { get; set; }
        /// <summary>
        /// ĐƠn vị quản lý
        /// </summary>
        public string UnitId { get; set; }
        /// <summary>
        /// Mức quyền
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// Cấp bậc
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Chức vụ
        /// </summary>
        public string Role { get; set; }
        /// <summary> 
        /// Điện thoại 
        /// </summary> 
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        /// <summary> 
        /// Mật khẩu 
        /// </summary> 
        public string Password { get; set; }
        /// <summary> 
        /// Mã hóa 
        /// </summary> 
        public string PasswordHash { get; set; }
        /// <summary> 
        /// Trạng thái 
        /// </summary> 
        public Nullable<int> Status { get; set; }
        /// <summary> 
        /// Mô tả 
        /// </summary> 
        public string Description { get; set; }

        public string ImageLink { get; set; }
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

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        /// <summary>
        /// Dánh sách quyền người dùng
        /// </summary>
        public List<FunctionModel> ListPermission { get; set; }

        //Log người dùng
        public string LogUserId { get; set; }
        public string ViolationEventId { get; set; }
    }
}
