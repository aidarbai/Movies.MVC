using System.Threading.Tasks;

namespace Cinema.BLL.Helpers.ChatPresence
{
    public interface IPresenceTracker
    {
        Task<ConnectionClosedResult> ConnectionClosed(string userId);
        Task<ConnectionOpenedResult> ConnectionOpened(string userId);
        Task<string[]> GetOnlineUsers();
    }
}