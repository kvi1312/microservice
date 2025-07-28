using AutoMapper;
using MediatR;
using Product.API.Repositories.Interface;
using Shared.DTOS;
using Shared.SeedWork;

namespace Product.API.Features.V1.Query.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ApiResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProduct(request.Id);
        if (product is null) return new ApiErrorResult<ProductDto>($"Could not find product with id {request.Id}");
        var result = _mapper.Map<ProductDto>(product);
        return new ApiSuccessResult<ProductDto>(result);
    }
}
