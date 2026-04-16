using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace B2B_Procurement___Order_Management_Platform.src.ArtMarket.Infrastructure.Data
{
    public class UserDb:DbContext
    {
        public UserDb(DbContextOptions<UserDb> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
