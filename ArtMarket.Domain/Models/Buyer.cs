using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models
{
    public class Buyer
    {
        public string BuyerId { get; set; }        
        public string BuyerDisplayName { get; set; }
        /// Relations
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
