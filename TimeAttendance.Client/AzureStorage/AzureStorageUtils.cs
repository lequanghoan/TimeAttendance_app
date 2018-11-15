using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Client.AzureStorage
{
    public static class AzureStorageUtils
    {
        public static string StorageAccountName { get; set; }
        public static string StorageAccountKey { get; set; }
        public static string StorageContainer { get; set; }
        public static string UrlHostImage { get; set; }

        public static CloudStorageAccount StorageAccount
        {
            get
            {
                string connectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", AzureStorageUtils.StorageAccountName, AzureStorageUtils.StorageAccountKey);
                return CloudStorageAccount.Parse(connectionString);
            }
        }
    }
}
