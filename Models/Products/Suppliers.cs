using System.ComponentModel.DataAnnotations;

namespace B2B_Procurement___Order_Management_Platform.Models.Products
{
    public class Suppliers
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [EmailAddress]
        public string ContactEmail { get; set; }
        public bool IsActive { get; set; }
        public virtual List<Product>? Products { get; set; }
    }
}
