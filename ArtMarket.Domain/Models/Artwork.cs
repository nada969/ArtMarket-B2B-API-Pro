using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models
{
    public class Artwork
    {
        public string ArtId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Boolean IsAvailable { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        /// Relations
        [ForeignKey("Artist")]
        public string ArtistId { get; set; }
        public Artist Artist{ get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
