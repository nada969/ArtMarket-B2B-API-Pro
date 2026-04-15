using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_Procurement___Order_Management_Platform.Models.Products
{
    public enum ProductStatus  { Active,Inactive,Discontinued }
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(25)]       
        public string Name { get; set; }

        public string Category { get; set; }

        public string CurrentStatus { get; set; }
        
        [ForeignKey("Supplier")]
        public int Supplier_Id { get;set; }
        public virtual Suppliers? Supplier { get; set; }

    }
}
