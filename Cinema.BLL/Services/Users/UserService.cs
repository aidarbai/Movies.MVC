using AutoMapper;
using Cinema.BLL.DTOs.User;
using Cinema.BLL.Helpers.Pagination;
using Cinema.COMMON.Filters;
using Cinema.COMMON.Responses;
using Cinema.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.BLL.Services.Users
{
    public class UserService : BaseService, IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public UserService(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResponseWithPayloadAndCount<List<ApplicationUserDTO>>> GetPaginatedUsersAsync(ViewFilter filter)
        {
            ResponseWithPayloadAndCount<List<ApplicationUserDTO>> response = new();

            try
            {
                var usersQuery = _userManager.Users.AsQueryable();

                if (!string.IsNullOrEmpty(filter.SearchString))
                {
                    usersQuery = usersQuery.Where(u => u.LastName.Contains(filter.SearchString)
                                           || u.FirstName.Contains(filter.SearchString));
                }

                if (!usersQuery.Any())
                {
                    response.Succeeded = false;
                    return response;
                }

                switch (filter.SortOrder)
                {
                    case "name":
                        usersQuery = usersQuery.OrderBy(u => u.FirstName);
                        break;
                    case "name_desc":
                        usersQuery = usersQuery.OrderByDescending(u => u.FirstName);
                        break;
                    case "lastname":
                        usersQuery = usersQuery.OrderBy(u => u.LastName);
                        break;
                    case "lastname_desc":
                        usersQuery = usersQuery.OrderByDescending(u => u.LastName);
                        break;
                    default:
                        break;
                }

                var result = PaginationHelper.GetPaginatedList(filter, usersQuery);

                var usersForView = new List<ApplicationUserDTO>();
                foreach (var user in result.Items)
                {
                    var userForView = Mapper.Map<ApplicationUserDTO>(user);
                    userForView.Roles = await GetUserRoles(user);
                    userForView.IsBanned = user.BannedOnDate.HasValue;
                    usersForView.Add(userForView);
                }

                response.Data = usersForView;
                response.TotalCount = result.PagesCount;
            }

            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = "An error occured while retrieving users list";
                response.Ex = ex;
            }

            return response;

        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
        public async Task<BaseResponse> UpdateUserAsync(ApplicationUserDTO userFromView)
        {
            BaseResponse response = new();
            try
            {
                var user = await _userManager.FindByIdAsync(userFromView.Id);
                if (user == null)
                {
                    response.Succeeded = false;
                    response.Message = "User not found";
                }

                IdentityResult result = new();

                if (userFromView.Roles?.Count > 0)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    result = await _userManager.RemoveFromRolesAsync(user, roles);
                    if (!result.Succeeded)
                    {
                        response.Succeeded = false;
                        response.Message = "Cannot remove user existing roles";
                    }
                    result = await _userManager.AddToRolesAsync(user, userFromView.Roles);
                    if (!result.Succeeded)
                    {
                        response.Succeeded = false;
                        response.Message = "Cannot add selected roles to user";
                    }
                }

                if (!string.IsNullOrEmpty(userFromView.Email) && userFromView.Email != user.Email)
                {
                    result = await _userManager.SetEmailAsync(user, userFromView.Email);
                    if (!result.Succeeded)
                    {
                        response.Succeeded = false;
                        response.Message = "Cannot update user's email";
                    }
                }

                user.FirstName = userFromView.FirstName;

                user.LastName = userFromView.LastName;

                if (userFromView.IsBanned && !user.BannedOnDate.HasValue)
                {
                    user.BannedOnDate = DateTime.Now;
                }
                else if (!userFromView.IsBanned && user.BannedOnDate.HasValue)
                {
                    user.BannedOnDate = null;
                }

                result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    response.Succeeded = false;
                    response.Message = "Cannot update user";
                }

                response.Message = "User has been updated successfully";
            }

            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = "An error occured while updating user";
                response.Ex = ex;
            }

            return response;
        }
        public async Task<BaseResponse> DeleteUserAsync(string id)
        {
            BaseResponse response = new();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    response.Succeeded = false;
                    response.Message = "User not found";
                }

                user.IsDeleted = true;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    response.Succeeded = false;
                    response.Message = "Cannot delete user";
                }

                response.Message = "User has been deleted successfully";
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
                response.Message = "An error occured while deleting the user";
            }

            return response;
        }
    }
}
