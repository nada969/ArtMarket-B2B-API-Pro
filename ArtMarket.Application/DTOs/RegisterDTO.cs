using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string email {get;set;}
        public string password { get; set; }
        public string userName { get; set; }
        public string role { get; set; }
        public RegisterDTO() { }
        public RegisterDTO(string email,string password,string role) 
        {
            this.email = email;
            this.password = password;
            this.role = role;
        }

    }
}
