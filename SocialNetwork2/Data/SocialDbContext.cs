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

    }
}
