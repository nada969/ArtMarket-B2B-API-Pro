using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string ArtistDisplayName { get; set; }
        public string Bio { get; set; }
        public string Status { get; set; }
        public string ProfileImage { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Subscription? Subscription { get; set; }
    }
}
