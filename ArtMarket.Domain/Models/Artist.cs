using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models
{
    public class Artist
    {
        public string ArtistId { get; set; }
        public string ArtistDisplayName { get; set; }
        public string Bio { get; set; }
        public Status Status { get; set; }
        public string ProfileImage { get; set; }
        /// Relations
        public string UserId { get; set; }
        public User User { get; set; }
        public Subscription? Subscription { get; set; }
        public ICollection<Artwork> Artworks { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
