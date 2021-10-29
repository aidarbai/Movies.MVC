using Cinema.COMMON.Constants;
using Cinema.COMMON.Responses;
using Cinema.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Cinema.BLL.Helpers.UserRoles
{
    public static class UserRolesHelper
    {
        public static BaseResponse CreateRoles(IServiceProvider serviceProvider)
        {
            BaseResponse response = new();
            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                
                Task<IdentityResult> roleResult;

                bool allRolesExisted = true;
                foreach (var roleName in AppConstants.Roles.Groups.ROLES)
                {
                    var roleExists = roleManager.RoleExistsAsync(roleName).Result;
                    if (!roleExists)
                    {
                        allRolesExisted = false;
                        roleResult = roleManager.CreateAsync(new IdentityRole(roleName));
                        roleResult.Wait();
                    }
                }
                if (allRolesExisted)
                {
                    response.Message = "All user roles already exist in database";
                }
                else
                {
                    response.Message = "User roles have been added successfully";
                }

                string email = configuration["superAdminEmail"];
                string password = configuration["superAdminPassword"];

                Task<ApplicationUser> testUser = userManager.FindByEmailAsync(email);
                testUser.Wait();

                if (testUser.Result == null)
                {
                    ApplicationUser superAdmin = new ();
                    superAdmin.Email = email;
                    superAdmin.UserName = email;

                    Task<IdentityResult> newUser = userManager.CreateAsync(superAdmin, password);
                    newUser.Wait();

                    if (newUser.Result.Succeeded)
                    {
                        Task<IdentityResult> newUserRole = userManager.AddToRoleAsync(superAdmin, AppConstants.Roles.SUPER_ADMIN);
                        newUserRole.Wait();
                        response.Message += ", superadmin has been added";
                    }
                }
                else
                {
                    response.Message += ", superadmin already exists in database";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
            }

            return response;

        }
    }
}
