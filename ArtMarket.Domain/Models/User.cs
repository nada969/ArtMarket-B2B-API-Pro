
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models
{
    public class User : IdentityUser
    {
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public Buyer? Buyer { get; set; }
        public Artist? Artist { get; set; }
    }
}
