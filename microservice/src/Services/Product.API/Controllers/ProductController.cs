using Infrastructure.Identity.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Services;
using Shared.Common.Constants;
using Shared.DTOS;
using System.ComponentModel.DataAnnotations;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("Bearer")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [ClaimsRequirementAttributes(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProducts()
    {
        var result = await _productService.GetAllProducts();
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ClaimsRequirementAttributes(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProductById(long id)
    {
        var result = await _productService.GetProduct(id);
        return Ok(result);
    }

    [HttpPost]
    [ClaimsRequirementAttributes(FunctionCode.PRODUCT, CommandCode.CREATE)]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductDto productDto)
    {
        var result = await _productService.AddProduct(productDto);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    [ClaimsRequirementAttributes(FunctionCode.PRODUCT, CommandCode.UPDATE)]
    public async Task<IActionResult> UpdateProduct(long id, [FromBody] UpdateProductDto productDto)
    {
        var result = await _productService.UpdateProduct(id, productDto);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    [ClaimsRequirementAttributes(FunctionCode.PRODUCT, CommandCode.DELETE)]
    public async Task<IActionResult> RemoveProduct([Required] long id)
    {
        var result = await _productService.RemoveProduct(id);
        return Ok(result);
    }

    [HttpGet("get-product-by-no/{productNo}")]
    [ClaimsRequirementAttributes(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProductByNo([Required] string productNo)
    {
        var result = await _productService.GetProductByNo(productNo);
        return Ok(result);
    }
}