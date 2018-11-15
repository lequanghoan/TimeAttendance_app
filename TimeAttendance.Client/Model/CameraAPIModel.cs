using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Client.Model
{
    public class InfoSettingModel : APIModel
    {
        public List<CameraModel> ListCamera { get; set; }
    }

    public class APIModel
    {
        public int ServerId { get; set; }
        public string ServiceBase { get; set; }
        public int TotalFrame { get; set; }
        public bool IsUpdate { get; set; }
        //Thông tin Queue
        public string QueueURL { get; set; }
        public string AccessKeyName { get; set; }
        public string AccessKeyValue { get; set; }
        public string BusTopicName { get; set; }
        public string BusKeySend { get; set; }
        //Thông tin Azure
        public string AzureName { get; set; }
        public string AzureKey { get; set; }
        public string AzureContainer { get; set; }
        public string AzureUrlHost { get; set; }
    }

    public class CameraModel
    {
        public string Index { get; set; }
        public string CameraId { get; set; }
        public string CameraType { get; set; }
        public string CameraIP { get; set; }
        public string CameraUser { get; set; }
        public string CameraPass { get; set; }
        public string StreaURI { get; set; }
        public string IndexView { get; set; }
        public uint BoxWidth { get; set; }
        public uint BoxHeight { get; set; }
        public uint BoxPointX { get; set; }
        public uint BoxPointY { get; set; }
    }

    public class CameraTypeModel
    {
        public string TypeId { get; set; }
        public string TypeName { get; set; }
    }
}
