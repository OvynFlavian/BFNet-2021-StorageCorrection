using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageCorrection.Services
{
    public interface ImageService
    {
        List<string> ReadAll();
        Task<List<string>> ReadAllAsync();
        string Upload(string fileName, byte[] fileData, string fileMimeType);
        Task<string> UploadAsync(string fileName, byte[] fileData, string fileMimeType);
    }
    public class ImageServiceImpl : ImageService
    {
        private string _AccessKey = string.Empty;
        private string _ContainerName = "images";
        
        public ImageServiceImpl(string accessKey)
        {
            _AccessKey = accessKey;
        }

        public List<string> ReadAll()
        {
            Task<List<string>> task = Task.Run(() => ReadAllAsync());
            task.Wait();
            return task.Result;
        }

        public async Task<List<string>> ReadAllAsync()
        {
            CloudBlobContainer blobContainer = await GetContainer();
            BlobContinuationToken continuationToken = null;

            List<string> uris = new List<string>();

            do
            {
                BlobResultSegment segment = await blobContainer.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, 50, continuationToken, null, null);
                foreach (CloudBlob blob in segment.Results)
                {
                    uris.Add(blob.Uri.AbsoluteUri);
                }
                continuationToken = segment.ContinuationToken;
            } while (continuationToken != null);

            return uris;
        }

        public string Upload(string fileName, byte[] fileData, string fileMimeType)
        {
            Task<string> task = Task.Run(() => UploadAsync(fileName, fileData, fileMimeType));
            task.Wait();
            return task.Result;
        }

        public async Task<string> UploadAsync(string fileName, byte[] fileData, string fileMimeType)
        {
            CloudBlobContainer blobContainer = await GetContainer();

            string strFileName = GenerateFileName(fileName);

            if (strFileName != null && fileData != null)
            {
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(strFileName);
                blockBlob.Properties.ContentType = fileMimeType;
                await blockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                return blockBlob.Uri.AbsoluteUri;
            }

            return string.Empty;
        }


        private string GenerateFileName(string strFileName)
        {
            string[] metadata = strFileName.Split(".");
            string day = $"{DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd")}";
            string time = $"{DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd\\THHmmssfff")}";

            return $"{day}/{time}.{metadata[metadata.Length - 1]}";
        }
        private async Task<CloudBlobContainer> GetContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_AccessKey);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(_ContainerName);

            if (await blobContainer.CreateIfNotExistsAsync())
            {
                await blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            return blobContainer;
        }
    }
}
