using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Utils
{
    public static class ErrorMessage
    {
        public const string SYS001 = "Không thể kết nối được với server Redis";
        public const string ERR001 = "Có lỗi phát sinh trong quá trình xử lý";

        public const string ERR002 = "Tài khoản đã tồn tại trong hệ thống. Xin vui lòng nhập tài khoản khác!";

        public const string ERR003 = "Tài khoản đã bị xóa bởi người dùng khác!";

        public const string ERR004 = "Nhóm người dùng của người dùng này đang bị khóa. Không thể kích hoạt người dùng này.";
        public const string ERR005 = "Nhóm quyền đã bị xóa bởi người dùng khác";
        public const string ERR006 = "Không thể xóa nhóm quyền này vì đã có người dùng gắn với nhóm quyền";

        public const string ERR007 = "Bản ghi này đã bị xóa bởi người dùng khác!";
        public const string ERR008 = "Không thể kết nối tới Service Bus";

    }
}
