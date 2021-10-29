using Cinema.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Cinema.BLL.Helpers.ChatPresence;
using System;
using System.Threading.Tasks;

namespace Cinema.MVC.Hubs
{
    [Authorize]
    public class ChatHub : BaseHub
    {
        private readonly IPresenceTracker presenceTracker;
        
        public ChatHub(IPresenceTracker presenceTracker, UserManager<ApplicationUser> _userManager) 
            : base (_userManager)
        {
            this.presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            string userFullName = await GetUserNameAsync(Context.User);
            var result = await presenceTracker.ConnectionOpened(userFullName);
            if (result.UserJoined)
            {
                await Clients.All.SendAsync("newMessage", "system", $"{userFullName} joined the chat");
            }

            var currentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("systemMessage", $"{string.Join(", ", currentUsers)}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userFullName = await GetUserNameAsync(Context.User);
            var result = await presenceTracker.ConnectionClosed(userFullName);
            if (result.UserLeft)
            {
                await Clients.All.SendAsync("newMessage", "system", $"{userFullName} left the chat");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            string userFullName = await GetUserNameAsync(Context.User);
            await Clients.All.SendAsync("newMessage", userFullName, message);
        }
    }
}