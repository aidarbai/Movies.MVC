using Cinema.BLL.DTOs.User;
using Cinema.COMMON.Filters;
using Cinema.COMMON.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema.BLL.Services.Users
{
    public interface IUserService
    {
        Task<BaseResponse> DeleteUserAsync(string id);
        Task<ResponseWithPayloadAndCount<List<ApplicationUserDTO>>> GetPaginatedUsersAsync(ViewFilter filter);
        Task<BaseResponse> UpdateUserAsync(ApplicationUserDTO userFromView);
    }
}