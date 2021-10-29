using Cinema.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinema.MVC.Hubs
{
    public class BaseHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BaseHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected async Task<string> GetUserNameAsync(ClaimsPrincipal user)
        {
            var requestedUser = await _userManager.GetUserAsync(user);
            return requestedUser.FirstName + " " + requestedUser.LastName;
        }
    }
}
