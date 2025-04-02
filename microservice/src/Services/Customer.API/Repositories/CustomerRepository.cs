using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interface;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories
{
    public class CustomerRepository : RepositoryQueryBase<Entities.Customer, int, CustomerContext>, ICustomerRepository
    {
        public CustomerRepository(CustomerContext dbContext) : base(dbContext)
        {
        }

        public async Task<Entities.Customer?> GetCustomerByUserName(string userName) => 
             await FindByCondition(x => string.Equals(x.UserName, userName)).SingleOrDefaultAsync();

        public async Task<IEnumerable<Entities.Customer>> GetCustomers() => await FindAll().ToListAsync();
    }
}
