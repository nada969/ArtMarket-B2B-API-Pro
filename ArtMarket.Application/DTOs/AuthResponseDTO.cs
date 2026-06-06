namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs
{
    public class AuthResponseDTO
    {
        public bool IsAuthenticated { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public DateTime ExpiresOn { get; set; }

    }
}
