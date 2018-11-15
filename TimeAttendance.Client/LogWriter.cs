using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TimeAttendance.Client
{
    public class LogWriter
    {
        private static LogWriter singletonObject;

        private LogWriter()
        { }

        public static LogWriter GetInstance()
        {
            if (singletonObject == null)
            {
                singletonObject = new LogWriter();
            }

            return singletonObject;
        }

        public async void Write(string message)
        {
            DateTime currently = DateTime.Now;
            string fileName = DateTime.Now.ToString("yyyyMMdd" + ".txt");

            StorageFolder storageFolder =
             ApplicationData.Current.LocalFolder;
            StorageFile sampleFile =
                            await storageFolder.CreateFileAsync(fileName,
                               CreationCollisionOption.OpenIfExists);

            try
            {
                await FileIO.AppendTextAsync(sampleFile, message);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
