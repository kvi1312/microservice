using Contracts.Common.Interfaces;
using Customer.API.Persistence;

namespace Customer.API.Repositories.Interface
{
    public interface ICustomerRepository : IRepositoryQueryBase<Entities.Customer, int, CustomerContext>
    {
        Task<IEnumerable<Entities.Customer>> GetCustomers();
        Task<Entities.Customer?> GetCustomerByUserName(string userName);
    }
}
