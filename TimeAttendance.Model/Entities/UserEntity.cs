using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class UserEntity
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public int Status { get; set; }
        public string Password { get; set; }
        public string ImageLink { get; set; }
        public string Id { get; set; }
        public string Code { get; set; }
        public string UnitId { get; set; }
        public string Type { get; set; }
        public string GroupId { get; set; }

        public string PoliceLevel { get; set; }

        public string Agency { get; set; }

        public List<string> ListPermission { get; set; }

        public string AuthorizeString { get; set; }
        public string AuthorizeItemString { get; set; }
        public UserEntity ListUser { get; set; }

        /// <summary>
        /// Trang chủ sau khi login thành công
        /// </summary>
        public string HomePage { get; set; }
        public string IsAdmin { get; set; }
        public string securityKey { get; set; }
    }
}
