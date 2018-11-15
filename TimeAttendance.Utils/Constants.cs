using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Utils
{
    public static class Constants
    {
        //Đang mở khóa
        public const int UnLock = 0;
        //Đang khóa
        public const int Lock = 1;
        //Chưa xóa
        public const int DeleteFalse = 0;
        //Đã xóa
        public const int DeleteTrue = 1;

        public const string PasswordDefault = "123456";

        //Hoàn thành
        public const int StatusFinish = 1;
        //Chưa hoàn thành
        public const int StatusUnFinish = 0;

        //Loại truy cập
        public const int LogTypeDataMining = 1;
        public const int LogTypeAccess = 0;

        //Cố định
        public const int MAX_RETURN_DATA_ROW = 100000;

        // Thư mục lưu trữ ảnh đại diện của user
        public const string FolderImageUser = "ImageUser";
        // Thư mục lưu trữ ảnh đại diện của user
        public const string FolderLogAttendance = "LogAttendance";
        // Thư mục lưu trữ ảnh đại diện của user
        public const string FolderImageFace = "ImageFace";

        //giới tính
        public const int Male = 1;
        public const int FeMale = 0;

        //nhóm fix
        public const string NhomNV = "67e88df6-63ca-4335-8533-ef5925fcadbe";
        public const string ImageEmployee = "ImageEmployee";
        public const string FolderEmployee = "ImageEmployee";

        public const int OK = 1;

        public const int ERROR = 0;

        public const int NOT_FOUND = 2;
        public const int DUPLICATE = 3;
        public const int USING = 4;

        public const string ConfigTimeIn = "TimeIn";
        public const string ConfigTimeOut = "TimeOut";
        public const string ConfigPercent = "Percent";
        public const string TimeAttendanceLog = "TimeAttendanceLog";

        //thành công gửi request
        public const string RequestOk = "200";
        //login cho AT
        public const string ATLogin = "AT:Login:";

    }
}
