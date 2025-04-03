using System.ComponentModel.DataAnnotations;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Product.API.Services;
using Shared.DTOS;

namespace Product.API.EndPoints;
public class ProductEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/products", GetProducts);
        app.MapPost("api/products", AddProduct);
        app.MapGet("api/products/{id:long}", GetProductById);
        app.MapPut("api/products/{id:long}", UpdateProduct);
        app.MapDelete("api/products/{id:long}", RemoveProduct);
        app.MapGet("api/products/get-product-by-no/{productNo}", GetProductByNo);
    }

    #region Delegate - Can reuse for unit test

    private async Task<IResult> GetProducts(
    IProductService productService)
    {
        return await productService.GetAllProducts();
    }

    private async Task<IResult> GetProductById(IProductService productService, long id)
    {
        return await productService.GetProduct(id);
    }

    private async Task<IResult> AddProduct(IProductService productService, [FromBody] CreateProductDto productDto)
    {
      return await productService.AddProduct(productDto);
    }

    private async Task<IResult> UpdateProduct(IProductService productService, long id, [FromBody] UpdateProductDto productDto)
    {
        return await productService.UpdateProduct(id, productDto);
    }

    private async Task<IResult> RemoveProduct(IProductService productService, [Required] long id)
    {
        return await productService.RemoveProduct(id);
    }

    private async Task<IResult> GetProductByNo(IProductService productService, [Required] string productNo)
    {
      return await productService.GetProductByNo(productNo);
    }
    #endregion
}