using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace B2B_Procurement___Order_Management_Platform.Models.Identity_Authorization
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = default!;
        public string Password { get; private set; } 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User(string password)
        {
            var hash = new PasswordHasher<User>();
            this.Password = hash.HashPassword(this, password);
        }
    }
}
