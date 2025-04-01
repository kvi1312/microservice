using Customer.API.Repositories.Interface;
using Customer.API.Services.Interfaces;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IResult> GetCustomerByUserNameAsync(string userName)
    {
        return Results.Ok(await _customerRepository.GetCustomerByUserName(userName));
    }

    public async Task<IResult> GetAllCustomers()
    {
        return Results.Ok(await _customerRepository.GetCustomers());
    }
}