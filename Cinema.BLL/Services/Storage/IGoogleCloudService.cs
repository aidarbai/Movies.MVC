using Cinema.COMMON.Responses;
using System.Threading.Tasks;

namespace Cinema.BLL.Services.Storage
{
    public interface IGoogleCloudService
    {
        Task DeleteFileAsync(string fileNameForStorage);
        Task<BaseResponse> UploadFileAsync(string url, string fileNameForStorage);
    }
}