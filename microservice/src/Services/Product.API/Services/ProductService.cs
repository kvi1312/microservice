using AutoMapper;
using Product.API.Entities;
using Product.API.Repositories.Interface;
using Shared.DTOS;

namespace Product.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IResult> GetAllProducts()
    {
        var product = await _productRepository.GetProducts();
        var result = _mapper.Map<IEnumerable<ProductDto>>(product);
        return Results.Ok(result);
    }

    public async Task<IResult> GetProduct(long id)
    {
        var product = await _productRepository.GetProduct(id);
        if (product is null) return Results.NotFound();
        var result = _mapper.Map<ProductDto>(product);
        return Results.Ok(result);
    }

    public async Task<IResult> AddProduct(CreateProductDto productDto)
    {
        var productEntity = await _productRepository.GetProductByNo(productDto.No);
        if (productEntity is not null) return Results.BadRequest($"Product No : {productDto.No} is existed");

        var product = _mapper.Map<CatalogProduct>(productDto);
        await _productRepository.CreateProduct(product);
        await _productRepository.SaveChangesAsync();
        var result = _mapper.Map<ProductDto>(product);
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateProduct(long id, UpdateProductDto productDto)
    {
        var product = await _productRepository.GetProduct(id);
        if (product is null) return Results.NotFound();

        var updatedProduct = _mapper.Map(productDto, product);
        await _productRepository.UpdateAsync(updatedProduct);
        await _productRepository.SaveChangesAsync();
        var result = _mapper.Map<ProductDto>(product);
        return Results.Ok(result);
    }

    public async Task<IResult> RemoveProduct(long id)
    {
        var product = await _productRepository.GetProduct(id);
        if (product is null) return Results.NotFound();
        await _productRepository.DeleteProduct(id);
        await _productRepository.SaveChangesAsync();
        return Results.Ok();
    }

    public async Task<IResult> GetProductByNo(string productNo)
    {
        var product = await _productRepository.GetProductByNo(productNo);
        if (product is null) return Results.NotFound();
        var result = _mapper.Map<ProductDto>(product);
        return Results.Ok(result);
    }
}