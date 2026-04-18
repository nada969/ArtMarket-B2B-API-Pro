using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models
{
    public class Subscription
    {
        public string SubscriptionId { get; set; }
        public SubscriptionTier Tier { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}
