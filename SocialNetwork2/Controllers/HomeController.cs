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


        public async Task<ActionResult> GetAllUsers()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var myrequests = _context.FriendRequests.Where(r => r.SenderId == user.Id);

            var users = await _context.Users
                .Where(u => u.Id != user.Id)
                .Select(u => new CustomIdentityUser
                {
                    Id = u.Id,
                    HasRequestPending = (myrequests.FirstOrDefault(r => r.ReceiverId == u.Id && r.Status == "Request") != null),
                    UserName=u.UserName,
                    IsOnline=u.IsOnline,
                    Image=u.Image,
                    Email=u.Email,  
                })
                .ToListAsync();

            return Ok(users);
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

        public async Task<ActionResult> GetAllRequests()
        {
            var current = await _userManager.GetUserAsync(HttpContext.User);
            var requests = _context.FriendRequests.Where(r => r.ReceiverId == current.Id);
            return Ok(requests);
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
