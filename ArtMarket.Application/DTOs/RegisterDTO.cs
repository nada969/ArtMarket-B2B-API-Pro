using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs
{
    public class RegisterDTO
    {
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
