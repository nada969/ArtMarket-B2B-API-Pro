using B2B_Procurement___Order_Management_Platform.Models.Products;
using B2B_Procurement___Order_Management_Platform.Repositories.Products;

namespace B2B_Procurement___Order_Management_Platform.Services.Products
{
    public interface IProductServices
    {
        IQueryable<Product> GetProducts();
        void CreateProduct(Product newproduct);
    }
    public class ProductServices:IProductServices
    {
        private readonly IProductRepository _productRepository;
        public ProductServices(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public IQueryable<Product> GetProducts()
        {
            var products = _productRepository.GetProducts();
            
            return products;
        }
        
        public void CreateProduct(Product newproduct)
        {
            _productRepository.CreateProduct(newproduct);
        }
    }
}
