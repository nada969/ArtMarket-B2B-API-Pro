using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs
{
    public class LoginDTO
    {
        public string email { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public LoginDTO(){}
        public LoginDTO(string email, string password, string token)
        {
            this.email = email;
            this.password = password;
            this.token = token;
        }
    }
}
