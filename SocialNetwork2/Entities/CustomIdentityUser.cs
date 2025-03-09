using Microsoft.AspNetCore.Identity;

namespace SocialNetwork2.Entities
{
    public class CustomIdentityUser:IdentityUser
    {
        public string? Image { get; set; }
        public bool IsOnline { get; set; }
        public DateTime DisConnectTime { get; set; } = DateTime.UtcNow;
        public string? ConnectTime { get; set; }
    }
}
