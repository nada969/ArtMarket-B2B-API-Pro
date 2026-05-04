using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string email {get;set;}

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain an uppercase letter, a number, and a special character.")]
        public string password { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(Buyer|Artist|Admin)$",
            ErrorMessage = "Role must be Buyer, Artist, or Admin.")]
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
