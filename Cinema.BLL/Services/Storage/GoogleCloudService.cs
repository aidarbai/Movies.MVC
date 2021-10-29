using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net;
using Microsoft.Extensions.Logging;
using Cinema.COMMON.Responses;
using System.Linq;

namespace Cinema.BLL.Services.Storage
{
    public class GoogleCloudService : IGoogleCloudService
    {
        private readonly GoogleCredential googleCredential;
        private readonly StorageClient storageClient;
        private readonly HttpClient _client;
        private readonly string bucketName;

        public GoogleCloudService(
            IConfiguration configuration,
            HttpClient client)
        {
            googleCredential = GoogleCredential.FromFile(configuration.GetValue<string>("GoogleCredentialsFile"));
            storageClient = StorageClient.Create(googleCredential);
            bucketName = configuration.GetValue<string>("GoogleCloudStorageBucket");
            _client = client;
        }

        public async Task<BaseResponse> UploadFileAsync(
            string url,
            string fileNameForStorage)
        {
            BaseResponse result = new();

            try
            {
                using var memoryStream = new MemoryStream();
                var response = await _client.GetAsync(url);

                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    await stream.CopyToAsync(memoryStream);
                }

                var dataObject = await storageClient.UploadObjectAsync(
                    bucketName, 
                    fileNameForStorage, 
                    null, 
                    memoryStream);
                result.Message = dataObject.MediaLink;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Message = "Error while uploading picture for movie";
                result.Ex = ex;
            }

            return result;
        }

        public async Task DeleteFileAsync(string fileNameForStorage)
        {
            var list = storageClient.ListObjects(bucketName).ToList();
            await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
        }
    }
}

