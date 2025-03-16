using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNetwork2.Entities;

namespace SocialNetwork2.Data
{
    public class SocialDbContext:IdentityDbContext<CustomIdentityUser,CustomIdentityRole,string>
    {
        public SocialDbContext(DbContextOptions<SocialDbContext> options)
            :base(options)
        {
        }

        public DbSet<Friend> Friends { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CustomIdentityUser>().Ignore(e=>e.IsFriend);
            builder.Entity<CustomIdentityUser>().Ignore(e=>e.HasRequestPending);
        }
    }
}
