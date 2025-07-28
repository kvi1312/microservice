using AutoMapper;
using MediatR;
using Product.API.Repositories.Interface;
using Shared.DTOS;
using Shared.SeedWork;

namespace Product.API.Features.V1.Query.GetAllProduct;

public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, ApiResult<IEnumerable<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    public GetAllProductQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    public async Task<ApiResult<IEnumerable<ProductDto>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProducts();
        var result = _mapper.Map<IEnumerable<ProductDto>>(product);
        return new ApiSuccessResult<IEnumerable<ProductDto>>(result);
    }
}
