using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TimeAttendance.Storage
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
                _container = ConfigurationManager.AppSettings["StorageContainer"];
                _urlHosting = ConfigurationManager.AppSettings["UrlHostImage"];
            }

            return singletonObject;
        }

        public StorageResult UploadPhoto(byte[] fileBinary, string contentType, string fileName)
        {
            StorageResult resultObject = new StorageResult();
            if (fileBinary == null || fileBinary.Length == 0)
            {
                return null;
            }

            try
            {
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(_container);

                if (container.CreateIfNotExists())
                {
                    // Enable public access on the newly created "images" container
                    container.SetPermissions(
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

                    blockBlob.UploadFromStream(stream);

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

        public async Task<string> UploadPhotoAsync(HttpPostedFile photoToUpload, string fileName, string folder)
        {
            if (photoToUpload == null || photoToUpload.ContentLength == 0)
            {
                return null;
            }

            string fullPath = null;

            try
            {
                // Create the blob client and reference the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_container);

                // Create a unique name for the images we are about to upload
                string imageName = folder + "/" + String.Format("{0}{1}",
                    Guid.NewGuid().ToString(),
                    Path.GetExtension(photoToUpload.FileName));

                // Upload image to Blob Storage
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageName);
                blockBlob.Properties.ContentType = photoToUpload.ContentType;
                await blockBlob.UploadFromStreamAsync(photoToUpload.InputStream);

                // Convert to be HTTP based URI (default storage path is HTTPS)
                fullPath = _urlHosting + _container + "/" + imageName;
            }
            catch (Exception ex)
            {
            }

            return fullPath;
        }

        public async Task<string> UploadImageStreamAsync(Stream file, string folder, string extension)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            string path = string.Empty;
            string folderSub = DateTime.Now.ToString("ddMMyyyy");
            try
            {
                // Create the blob client and reference the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
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

        public async Task DownloadPhotoAsync(string filePath)
        {

            try
            {
                // Create the blob client and reference the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_container);

                // Upload image to Blob Storage
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);
                await blockBlob.DownloadToFileAsync(filePath, FileMode.Create);
            }
            catch (Exception ex)
            {
            }
        }

        public Stream ReadFileToStream(string filePath)
        {

            try
            {
                // Create the blob client and reference the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_container);

                // Upload image to Blob Storage
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);
                return blockBlob.OpenRead();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task DeletePhotoAsync(string filePath)
        {
            try
            {
                // Create the blob client and reference the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_container);

                // Upload image to Blob Storage
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);
                await blockBlob.DeleteIfExistsAsync();

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Check tồn tại file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool CheckExitPhotoAsync(string filePath)
        {
            try
            {
                // Create the blob client and reference the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_container);

                // Upload image to Blob Storage
                CloudBlob blob = container.GetBlobReference(filePath);
                bool tem = blob.Exists();
                return blob.Exists();

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
