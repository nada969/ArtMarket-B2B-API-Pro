using B2B_Procurement___Order_Management_Platform.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace B2B_Procurement___Order_Management_Platform.Data
{
    public class ProductDb:DbContext
    {
        public ProductDb(DbContextOptions<ProductDb> options)
            :base (options) { }

        public DbSet<Product>   Products { get; set; }
    }
}
