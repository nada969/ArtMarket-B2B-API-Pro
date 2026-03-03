using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_Procurement___Order_Management_Platform.Models.Identity_Authorization
{
    public class UserRole
    {
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        [ForeignKey("Role")]
        public string RoleName { get; set; }
        public virtual Role? Role { get; set; }
    }
}
