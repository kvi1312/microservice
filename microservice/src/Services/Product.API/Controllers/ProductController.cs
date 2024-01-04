using Microsoft.AspNetCore.Mvc;
using Product.API.Repositories.Interface;

namespace Product.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var product = await _productRepository.GetProducts();
            return Ok(product);
        }
    }
}
