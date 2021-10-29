using Cinema.BLL.DTOs.User;
using Cinema.BLL.Services.Users;
using Cinema.COMMON.Constants;
using Cinema.COMMON.Filters;
using Cinema.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.MVC.Controllers
{
    [Authorize(Roles = AppConstants.Roles.Groups.ADMINSGROUP)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserService userService,
            ILogger<UserController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(ViewFilter filter)
        {
            filter.PageSize = filter.PageSize <= 1 ? 5 : filter.PageSize;

            ViewData["CurrentSort"] = filter.SortOrder;
            ViewData["FirstNameSortParm"] = filter.SortOrder == "name" ? "name_desc" : "name";
            ViewData["LastNameSortParm"] = string.IsNullOrEmpty(filter.SortOrder) ? "lastname_desc" : "lastname";

            if (filter.SearchString != null)
            {
                filter.PageNumber = 1;
            }
            else
            {
                filter.SearchString = filter.CurrentFilter;
            }

            ViewData["CurrentFilter"] = filter.SearchString;

            var response = await _userService.GetPaginatedUsersAsync(filter);

            if (response.Succeeded)
            {
                ViewBag.Users = response.Data;
                ViewBag.TotalPages = response.TotalCount;
                ViewBag.PageNumber = filter.PageNumber;
                ViewBag.PageSize = filter.PageSize;

                var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                ViewBag.AllRoles = allRoles;
            }

            else if (response.Message != null && response.Ex != null)
            {
                _logger.LogError(response.Message, response.Ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(ApplicationUserDTO userFromView)
        {
            if (userFromView.Id == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                ViewBag.Message = "Incorrect input";
                return View(nameof(Index));
            }

            var response = await _userService.UpdateUserAsync(userFromView);

            if (!response.Succeeded && response.Ex != null)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
            }

            TempData["Message"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var response = await _userService.DeleteUserAsync(id);
            if (!response.Succeeded && response.Ex != null)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
            }

            TempData["Message"] = response.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
