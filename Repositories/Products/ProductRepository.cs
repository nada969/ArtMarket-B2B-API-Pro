using B2B_Procurement___Order_Management_Platform.Data;
using B2B_Procurement___Order_Management_Platform.Models.Products;
namespace B2B_Procurement___Order_Management_Platform.Repositories.Products
{
    public interface IProductRepository
    {
        IQueryable<Product> GetProducts();
        void CreateProduct( Product newproduct);
    }
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDb _productDb;
        public ProductRepository(ProductDb productDb)
        {
            _productDb = productDb;
        } 
        public IQueryable<Product> GetProducts()
        {
            return _productDb.Products;
        }
        public void CreateProduct(Product newproduct) 
        {
            _productDb.Products.Add(newproduct);
            _productDb.SaveChanges();
        }
    }
}
