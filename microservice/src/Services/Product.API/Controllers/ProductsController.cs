using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Product.API.Entities;
using Product.API.Repositories.Interface;
using Shared.DTOS;
using System.ComponentModel.DataAnnotations;

namespace Product.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var product = await _productRepository.GetProducts();
            var result = _mapper.Map<IEnumerable<ProductDto>>(product);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetProducts([Required] long id)
        {
            var product = await _productRepository.GetProduct(id);
            if (product == null) return NotFound();

            var result = _mapper.Map<ProductDto>(product);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            var productEntity = await _productRepository.GetProductByNo(productDto.No);
            if (productEntity !=null) return BadRequest($"Product No : {productDto.No} is existed");

            var product = _mapper.Map<CatalogProduct>(productDto);
            await _productRepository.CreateProduct(product);
            await _productRepository.SaveChangesAsync();
            var result = _mapper.Map<ProductDto>(product);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateProduct([Required] long id, [FromBody] UpdateProductDto productDto)
        {
            var product = await _productRepository.GetProduct(id);
            if (product == null) return NotFound();

            var updatedProduct = _mapper.Map(productDto, product); // map to new obj from source obj
            await _productRepository.UpdateAsync(updatedProduct);
            await _productRepository.SaveChangesAsync();
            var result = _mapper.Map<ProductDto>(product); //map an objec to mactch result type
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteProduct([Required] long id)
        {
            var product = await _productRepository.GetProduct(id);
            if (product == null) return NotFound();

            await _productRepository.DeleteProduct(id);
            await _productRepository.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Additional Resources
        [HttpGet("get-product-by-no/{productNo}")]
        public async Task<IActionResult> GetProductByNo([Required] string productNo)
        {
            var product = await _productRepository.GetProductByNo(productNo);
            if (product == null) return NotFound();
            var result = _mapper.Map<ProductDto>(product);
            return Ok(result);
        }
        #endregion  
    }
}
