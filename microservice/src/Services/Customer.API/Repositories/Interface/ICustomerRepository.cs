using Contracts.Common.Interfaces;
using Customer.API.Persistence;

namespace Customer.API.Repositories.Interface
{
    public interface ICustomerRepository : IRepositoryBaseAsync<Entities.Customer, int, CustomerContext>
    {
        Task<IEnumerable<Entities.Customer>> GetCustomers();
        Task<Entities.Customer> GetCustomerById(long id);
        Task CreateCustomer(Entities.Customer customer);
        Task UpdateCustomer(Entities.Customer customer);
        Task DeleteCustomer(long id);
        Task<Entities.Customer?> GetCustomerByUserName(string userName);
    }
}
