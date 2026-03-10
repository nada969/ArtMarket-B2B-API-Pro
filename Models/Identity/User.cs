//using Microsoft.AspNetCore.Identity;
//using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace B2B_Procurement___Order_Management_Platform.Models.Identity
{
    public class User 
    {
        public int Id { get; set; }
        //[RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$")]
        [EmailAddress]
        public string Email { get; set; } = default!;
        public string Password { get; private set; } 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User(string email,string password)
        {
            if (string.IsNullOrWhiteSpace(email)) { throw new ArgumentException(" Enter valid email"); }
            if (string.IsNullOrWhiteSpace(password)) { throw new ArgumentException(" Enter valid password "); }

            Email = email;
            Password =  password;
        }
    }
}
