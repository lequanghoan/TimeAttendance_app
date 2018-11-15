using Newtonsoft.Json;
using ppatierno.AzureSBLite;
using ppatierno.AzureSBLite.Messaging;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace TimeAttendance.Client.ServiceBus
{
    public class ServiceBusFuntion<T>
    {
        public async Task SendMessagesAsync(T messageObject)
        {
            try
            {
                string jsonMessage = JsonConvert.SerializeObject(messageObject);
                await SendMessageToQ(ServiceBusSetting.QueueURL, ServiceBusSetting.AccessKeyName, ServiceBusSetting.AccessKeyValue, jsonMessage);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Gửi message lên queue
        /// </summary>
        /// <param name="queueURL"></param>
        /// <param name="keyName"></param>
        /// <param name="keyValue"></param>
        /// <param name="body"></param>
        public async Task<bool> SendMessageToQ(string queueURL, string keyName, string keyValue, string jsonMessage)
        {
            var sasToken = GetSASToken(queueURL, keyName, keyValue);
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sasToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent streamContent = new StringContent(jsonMessage);
                var response =  httpClient.PostAsync(string.Format("{0}/messages?visibilitytimeout=30&timeout=30", queueURL), streamContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Token
        /// </summary>
        /// <param name="resourceUri"></param>
        /// <param name="keyName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetSASToken(string resourceUri, string keyName, string key)
        {
            var expiry = GetExpiry();
            string stringToSign = WebUtility.UrlEncode(resourceUri) + "\n" + expiry;
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));

            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                WebUtility.UrlEncode(resourceUri), WebUtility.UrlEncode(signature), expiry, keyName);
            return sasToken;
        }

        private static string GetExpiry()
        {
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Convert.ToString((int)sinceEpoch.TotalSeconds + 3600);
        }

    }
}
