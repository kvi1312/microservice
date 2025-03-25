using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interface;
using Infrastructure.Common;

namespace Customer.API.Repositories
{
    public class CustomerRepository : RepositoryBaseAsync<Entities.Customer, int, CustomerContext>, ICustomerRepository
    {
        public CustomerRepository(CustomerContext dbContext, IUnitOfWork<CustomerContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        public Task CreateCustomer(Entities.Customer customer)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCustomer(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Entities.Customer> GetCustomerById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Entities.Customer>> GetCustomers()
        {
            throw new NotImplementedException();
        }

        public Task UpdateCustomer(Entities.Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
