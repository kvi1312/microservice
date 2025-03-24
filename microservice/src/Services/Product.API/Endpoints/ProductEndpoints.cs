using AutoMapper;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Product.API.Entities;
using Product.API.Repositories.Interface;
using Shared.DTOS;
using System.ComponentModel.DataAnnotations;
namespace Product.API.EndPoints;
public class ProductEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // TODO : Write Extension having MapGroup feature like .NET 8.0
        app.MapGet("api/products", GetProducts);
        app.MapPost("api/products", AddProduct);
        app.MapGet("api/products/{id:long}", GetProductById);
        app.MapPut("api/products/{id:long}", UpdateProduct);
        app.MapDelete("api/products/{id:long}", RemoveProduct);
        app.MapGet("api/products/get-product-by-no/{productNo}", GetProductByNo);
    }

    #region Delegate - Can reuse for unit test

    private static async Task<IResult> GetProducts(
    IProductRepository productRepository,
    IMapper mapper)
    {
        var product = await productRepository.GetProducts();
        var result = mapper.Map<IEnumerable<ProductDto>>(product);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProductById(IProductRepository productRepository, IMapper mapper, long id)
    {
        var product = await productRepository.GetProduct(id);
        if (product is null) return Results.NotFound();
        var result = mapper.Map<ProductDto>(product);
        return Results.Ok(result);
    }

    private static async Task<IResult> AddProduct(IProductRepository productRepository, IMapper mapper, [FromBody] CreateProductDto productDto)
    {
        var productEntity = await productRepository.GetProductByNo(productDto.No);
        if (productEntity is not null) return Results.BadRequest($"Product No : {productDto.No} is existed");

        var product = mapper.Map<CatalogProduct>(productDto);
        await productRepository.CreateProduct(product);
        await productRepository.SaveChangesAsync();
        var result = mapper.Map<ProductDto>(product);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateProduct(IProductRepository productRepository, IMapper mapper, long id, [FromBody] UpdateProductDto productDto)
    {
        var product = await productRepository.GetProduct(id);
        if (product is null) return Results.NotFound();

        var updatedProduct = mapper.Map(productDto, product);
        await productRepository.UpdateAsync(updatedProduct);
        await productRepository.SaveChangesAsync();
        var result = mapper.Map<ProductDto>(product);
        return Results.Ok(result);
    }

    private static async Task<IResult> RemoveProduct(IProductRepository productRepository, IMapper mapper, [Required] long id)
    {
        var product = await productRepository.GetProduct(id);
        if (product is null) return Results.NotFound();
        await productRepository.DeleteProduct(id);
        await productRepository.SaveChangesAsync();
        return Results.Ok();
    }

    private static async Task<IResult> GetProductByNo(IProductRepository productRepository, IMapper mapper, [Required] string productNo)
    {
        var product = await productRepository.GetProductByNo(productNo);
        if (product is null) return Results.NotFound();
        var result = mapper.Map<ProductDto>(product);
        return Results.Ok(result);
    }
    #endregion
}