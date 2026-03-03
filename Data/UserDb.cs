using B2B_Procurement___Order_Management_Platform.Models.Identity_Authorization;
using Microsoft.EntityFrameworkCore;

namespace B2B_Procurement___Order_Management_Platform.Data
{
    public class UserDb:DbContext
    {
        public UserDb(DbContextOptions<UserDb> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
    }
}
