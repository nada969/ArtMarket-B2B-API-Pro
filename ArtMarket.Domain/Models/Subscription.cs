namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}
