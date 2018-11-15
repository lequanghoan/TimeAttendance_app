using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TimeAttendance.Client.AzureStorage
{
    public class AzureStorageUploadFiles
    {

        private static AzureStorageUploadFiles singletonObject;

        private static CloudStorageAccount storageAccount;
        private static string _container;
        private static string _urlHosting;

        public static AzureStorageUploadFiles GetInstance()
        {
            if (singletonObject == null)
            {
                singletonObject = new AzureStorageUploadFiles();
                storageAccount = AzureStorageUtils.StorageAccount;
                _container = AzureStorageUtils.StorageContainer;
                _urlHosting = AzureStorageUtils.UrlHostImage;
            }

            return singletonObject;
        }

        public async Task<StorageResult> UploadImageBinaryAsync(byte[] fileBinary, string contentType, string fileName)
        {
            StorageResult resultObject = new StorageResult();
            if (fileBinary == null || fileBinary.Length == 0)
            {
                return null;
            }

            try
            {
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                var serviceProperties = await blobClient.GetServicePropertiesAsync();
                serviceProperties.Cors.CorsRules.Clear();
                serviceProperties.Cors.CorsRules.Add(new CorsRule
                {
                    AllowedHeaders = new List<string> { "*" },
                    AllowedMethods = CorsHttpMethods.Get | CorsHttpMethods.Head,
                    AllowedOrigins = new List<string> { "*" },
                    ExposedHeaders = new List<string> { "*" }
                });
                await blobClient.SetServicePropertiesAsync(serviceProperties);
                CloudBlobContainer container = blobClient.GetContainerReference(_container);

                if (await container.CreateIfNotExistsAsync())
                {
                    // Enable public access on the newly created "images" container
                    await container.SetPermissionsAsync(
                         new BlobContainerPermissions
                         {
                             PublicAccess =
                                 BlobContainerPublicAccessType.Blob
                         });
                }

                using (var stream = new MemoryStream(fileBinary, writable: false))
                {
                    // Upload image to Blob Storage
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                    blockBlob.Properties.ContentType = contentType;

                    await blockBlob.UploadFromStreamAsync(stream);

                    resultObject.FileName = fileName;
                    resultObject.Folder = _container;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return resultObject;
        }

        public async Task<string> UploadImageStreamAsync(Stream file, string folder, string extension)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            string path = string.Empty;
            string folderSub = DateTime.Now.ToString("ddMMyyyy") + "/" + DateTime.Now.ToString("HH");
            try
            {
                // Create the blob client and reference the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                var serviceProperties = await blobClient.GetServicePropertiesAsync();
                serviceProperties.Cors.CorsRules.Clear();
                serviceProperties.Cors.CorsRules.Add(new CorsRule
                {
                    AllowedHeaders = new List<string> { "*" },
                    AllowedMethods = CorsHttpMethods.Get | CorsHttpMethods.Head,
                    AllowedOrigins = new List<string> { "*" },
                    ExposedHeaders = new List<string> { "*" }
                });
                await blobClient.SetServicePropertiesAsync(serviceProperties);
                CloudBlobContainer container = blobClient.GetContainerReference(_container);

                // Create a unique name for the images we are about to upload
                string imageName = folder + "/" + folderSub + "/" + String.Format("{0}.{1}",
                    Guid.NewGuid().ToString(),
                    extension);

                // Upload image to Blob Storage
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageName);
                blockBlob.Properties.ContentType = String.Format("image/{0}",
                    Guid.NewGuid().ToString(),
                    extension);
                await blockBlob.UploadFromStreamAsync(file);

                // Convert to be HTTP based URI (default storage path is HTTPS)
                path = imageName;
            }
            catch (Exception ex)
            {
            }

            return path;
        }
    }
}
