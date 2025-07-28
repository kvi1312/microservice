using AutoMapper;
using Infrastructure.Identity.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Features.V1.Query.GetAllProduct;
using Product.API.Features.V1.Query.GetProduct;
using Product.API.Features.V1.Query.GetProductByNo;
using Product.API.Repositories.Interface;
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
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    public ProductController(IProductService productService, IProductRepository productRepository, IMapper mapper, IMediator mediator)
    {
        _productService = productService;
        _productRepository = productRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet]
    [ClaimsRequirementAttributes(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProducts()
    {
        var products = new GetAllProductQuery();
        var result = await _mediator.Send(products);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ClaimsRequirementAttributes(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProductById(long id)
    {
        var product = new GetProductQuery(id);
        var result = await _mediator.Send(product);
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
        var product = new GetProductByNoQuery(productNo);
        var result = await _mediator.Send(product);
        return Ok(result);
    }
}