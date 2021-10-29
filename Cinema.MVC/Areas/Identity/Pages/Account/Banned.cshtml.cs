using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinema.COMMON.Constants;
using Cinema.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cinema.MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class BannedModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BannedModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string Emails { get; set; }
        public void OnGet()
        {
            var admins = _userManager.GetUsersInRoleAsync(AppConstants.Roles.ADMIN).Result.Select(u => u.Email);
            Emails = string.Join(", ", admins);
        }
    }
}
