using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Product.API.Repositories.Interface;
using Shared.DTOS;
using Shared.SeedWork;

namespace Product.API.Features.V1.Query.GetProductByNo;

public class GetProductByNoQueryHandler : IRequestHandler<GetProductByNoQuery, ApiResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByNoQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<ProductDto>> Handle(GetProductByNoQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByNo(request.ProductNo);
        if (product is null) return new ApiErrorResult<ProductDto>($"Could not find product with item No {request.ProductNo}");
        var result = _mapper.Map<ProductDto>(product);
        return new ApiSuccessResult<ProductDto>(result);
    }
}
