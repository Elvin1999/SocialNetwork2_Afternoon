using Microsoft.AspNetCore.Identity;

namespace SocialNetwork2.Entities
{
    public class CustomIdentityUser:IdentityUser
    {
        public string? Image { get; set; }
        public bool IsOnline { get; set; }
        public bool IsFriend { get; set; }
        public bool HasRequestPending { get; set; }
        public DateTime DisConnectTime { get; set; } = DateTime.UtcNow;
        public string? ConnectTime { get; set; }
        public List<Friend> Friends { get; set; }
        public List<Chat> Chats { get; set; }
        public List<FriendRequest> FriendRequests { get; set; }
        public CustomIdentityUser()
        {
            Chats = new List<Chat>();
            Friends = new List<Friend>();
            FriendRequests = new List<FriendRequest>();
        }
    }
}
