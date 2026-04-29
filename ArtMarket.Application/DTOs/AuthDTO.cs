namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs
{
    public class AuthDTO
    {
        public bool Success { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }

        public static AuthDTO Ok(string email, string token) => new()
        { Success = true, Email = email, Token = token };
        public static AuthDTO Fail(string message) => new()
        { Success = false, Message = message };
    }
}
