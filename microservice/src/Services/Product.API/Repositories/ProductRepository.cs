using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Product.API.Entities;
using Product.API.Persistence;
using Product.API.Repositories.Interface;

namespace Product.API.Repostiories
{
    public class ProductRepository : RepositoryBase<CatalogProduct, long, ProductContext>, IProductRepository
    {
        public ProductRepository(ProductContext dbContext, IUnitOfWork<ProductContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        public Task CreateProduct(CatalogProduct product) => CreateAsync(product);

        public async Task DeleteProduct(long id)
        {
            var product = await GetProduct(id);
            if (product is not null) await DeleteAsync(product);
        }

        public async Task<IEnumerable<CatalogProduct>> GetProducts() => await FindAll().ToListAsync();

        public Task<CatalogProduct> GetProductByNo(string productNo) => FindByCondition(b => b.No.Equals(productNo)).SingleOrDefaultAsync();

        public Task<CatalogProduct> GetProduct(long id) => GetByIdAsync(id);

        public Task UpdateProduct(CatalogProduct product) => UpdateAsync(product);
    }
}
