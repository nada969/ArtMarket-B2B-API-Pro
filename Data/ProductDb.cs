using B2B_Procurement___Order_Management_Platform.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace B2B_Procurement___Order_Management_Platform.Data
{
    public class ProductDb:DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Suppliers> suppliers { get; set; }
        public ProductDb(DbContextOptions<ProductDb> options)
            :base (options) { }

    }
}
