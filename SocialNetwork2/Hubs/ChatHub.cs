using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialNetwork2.Data;
using SocialNetwork2.Entities;

namespace SocialNetwork2.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly SocialDbContext _context;
        private IHttpContextAccessor _contextAccessor;

        public ChatHub(UserManager<CustomIdentityUser> userManager, SocialDbContext context, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            var userItem = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            userItem.IsOnline = true;
            _context.Update(userItem);
            await _context.SaveChangesAsync();

            string info = user.UserName + " connected successfully";
            await Clients.Others.SendAsync("Connect", info);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            var userItem = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            userItem.IsOnline = false;
            _context.Update(userItem);
            await _context.SaveChangesAsync();

            string info = user.UserName + " disconnected successfully";
            await Clients.Others.SendAsync("Disconnect", info);
        }

        public async Task GetMessages(string receiverId, string senderId)
        {
            await Clients.Users(new String[] { receiverId, senderId }).SendAsync("ReceiveMessages", receiverId, senderId);
            await Clients.User(receiverId).SendAsync("GetSound");
        }
        public async Task SendFollow(string id)
        {
            await Clients.User(id).SendAsync("ReceiveNotification");
        }

    }
}
