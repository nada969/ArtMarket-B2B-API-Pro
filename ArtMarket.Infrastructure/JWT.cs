namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure
{
    public class JWT
    {
        public string Key { get; set; } 
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int DurationInDays { get; set; }
    }
}
