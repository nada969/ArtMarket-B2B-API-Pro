using B2B_Procurement___Order_Management_Platform.Data;
using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Application.Services.Products;
using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Domain.Models.Products;
using Microsoft.AspNetCore.Mvc;

namespace B2B_Procurement___Order_Management_Platform.src.ArtMarket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;
        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        [HttpGet]
        public IActionResult GetProduct()
        {
            var products = _productServices.GetProducts();
            if(products == null) { return NotFound(); }
            return Ok(products);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetProductId(int id)
        {
            var product = _productServices.GetProducts().FirstOrDefault(x => x.Id == id);
            if (product == null) { return NotFound(); }
            return Ok(product);
        }

        [HttpPost(Name =("ProductDetails"))]
        public IActionResult CreateProduct([FromBody] Product newproduct)
        {
            if (ModelState.IsValid)
            {
                _productServices.CreateProduct(newproduct);
                string url = Url.Link("ProductDetails", new { index = newproduct.Id}) ;
                return Created(url,newproduct);
            }
            return BadRequest(ModelState);
        }
    }
}
