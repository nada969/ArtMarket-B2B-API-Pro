using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public Status Status { get; set; }
        /// Relations
        [ForeignKey("ArtWork")]
        public string ArtId { get; set; }
        public Artwork ArtWork { get; set; }
        [ForeignKey("Buyer")]
        public string BuyerId { get; set; }
        public Buyer Buyer { get; set; }
        [ForeignKey("Artist")]
        public string ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}
