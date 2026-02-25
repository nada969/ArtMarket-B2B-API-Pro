using System;
using System.ComponentModel.DataAnnotations;

namespace B2B_Procurement___Order_Management_Platform.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }
        public string Category { get; set; }
        public string Supplier { get; set; }
//- Current status(Active, Inactive, Discontinued)
    }
}
