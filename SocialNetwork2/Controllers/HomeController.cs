using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork2.Data;
using SocialNetwork2.Entities;
using SocialNetwork2.Models;
using System.Diagnostics;

namespace SocialNetwork2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly SocialDbContext _context;
        public HomeController(ILogger<HomeController> logger, UserManager<CustomIdentityUser> userManager, SocialDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = user;

            return View();
        }

        public async Task<ActionResult> AcceptRequest(string senderId, string receiverId, int requestId)
        {
            var sender = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == senderId);
            var receiver = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == receiverId);

            if (receiver != null && sender != null)
            {
                await _context.FriendRequests.AddAsync(new FriendRequest
                {
                    Content = $"{receiver.UserName} accepted friend request at {DateTime.Now.ToShortDateString()}-{DateTime.Now.ToShortTimeString()}",
                    SenderId = receiver.Id,
                    ReceiverId = sender.Id,
                    Sender = sender,
                    Status = "Notification"
                });

                var request = await _context.FriendRequests.FirstOrDefaultAsync(r => r.Id == requestId);
                if (request != null)
                {
                    _context.FriendRequests.Remove(request);
                }
                _context.Friends.Add(new Friend
                {
                    OwnId = sender.Id,
                    YourFriendId = receiver.Id
                });
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
        }

        public async Task<ActionResult> GetAllUsers()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var myrequests = _context.FriendRequests.Where(r => r.SenderId == user.Id);

            var myfriends = _context.Friends.Where(f => f.OwnId == user.Id || f.YourFriendId == user.Id);

            var users = await _context.Users
                .Where(u => u.Id != user.Id)
                .Select(u => new CustomIdentityUser
                {
                    Id = u.Id,
                    HasRequestPending = (myrequests.FirstOrDefault(r => r.ReceiverId == u.Id && r.Status == "Request") != null),
                    IsFriend = myfriends.FirstOrDefault(f => f.OwnId == u.Id || f.YourFriendId == u.Id) != null,
                    UserName = u.UserName,
                    IsOnline = u.IsOnline,
                    Image = u.Image,
                    Email = u.Email,
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpDelete]
        public async Task<ActionResult> UnFollow(string receiverId)
        {
            var sender = await _userManager.GetUserAsync(HttpContext.User);
            var receiver = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == receiverId);

            var friend = await _context.Friends.FirstOrDefaultAsync(f => f.OwnId == sender.Id && f.YourFriendId == receiver.Id ||
            f.OwnId == receiver.Id && f.YourFriendId == sender.Id);
            _context.Friends.Remove(friend);

            _context.FriendRequests.Add(new FriendRequest
            {
                Content = $"{sender.UserName} unfollowed you",
                SenderId = sender.Id,
                ReceiverId = receiverId,
                Status = "Notification"
            });

            await _context.SaveChangesAsync();
            return Ok();
        }
        public async Task<ActionResult> SendFollow(string id)
        {
            var sender = await _userManager.GetUserAsync(HttpContext.User);
            var receiverUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (receiverUser != null)
            {
                _context.FriendRequests.Add(new FriendRequest
                {
                    Content = $"{sender.UserName} sent friend request at {DateTime.Now.ToLongDateString()}",
                    SenderId = sender.Id,
                    Sender = sender,
                    ReceiverId = receiverUser.Id,
                    Status = "Request"
                });

                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteRequest(int id)
        {
            var request = await _context.FriendRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return NotFound();
            _context.FriendRequests.Remove(request);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> TakeRequest(string id)
        {
            var current = await _userManager.GetUserAsync(HttpContext.User);
            var request = await _context.FriendRequests.FirstOrDefaultAsync(r => r.SenderId == current.Id && r.ReceiverId == id);
            if (request == null) return NotFound();
            _context.FriendRequests.Remove(request);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<ActionResult> GetAllRequests()
        {
            var current = await _userManager.GetUserAsync(HttpContext.User);
            var requests = _context.FriendRequests.Where(r => r.ReceiverId == current.Id);
            return Ok(requests);
        }

        public async Task<ActionResult> DeclineRequest(int id, string senderId)
        {
            var current = await _userManager.GetUserAsync(HttpContext.User);
            var request = await _context.FriendRequests.FirstOrDefaultAsync(f => f.Id == id);
            _context.FriendRequests.Remove(request);

            _context.FriendRequests.Add(new FriendRequest
            {
                Content = $"{current.UserName} declined your friend request at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}",
                SenderId = current.Id,
                Sender = current,
                ReceiverId = senderId,
                Status = "Notification"
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
