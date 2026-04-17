using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models
{
    public class Buyer
    {
        public int BuyerId { get; set; }        
        public string BuyerDisplayName { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
