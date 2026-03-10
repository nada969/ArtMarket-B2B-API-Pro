using System.ComponentModel.DataAnnotations;

namespace B2B_Procurement___Order_Management_Platform.Models.Identity
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression("(Admin|Buyer|Sales)")]
        public string? Name { get; set; }
    }
}
