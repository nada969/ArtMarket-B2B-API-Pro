using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_Procurement___Order_Management_Platform.Models.Products
{
    public enum ProductStatus  { Active,Inactive,Discontinued }
    public class Product
    {
        [Column("Id")]
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(25)]       
        [Column("Name")]
        public string Name { get; set; }

        [Column("Category")]
        public string Category { get; set; }

        [Column("Supplier")]
        public string Supplier { get; set; }

        [Column("CurrentStatus")]
        public string CurrentStatus { get; set; }
    }
}
