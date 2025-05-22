using Carter;
using Customer.API.Services.Interfaces;

namespace Customer.API.Endpoints;

public class CustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/customer", GetAllCustomers);
        app.MapGet("api/customer/{userName}", GetCustomerByUserName);
    }

    #region Delegate Funcs
    private async Task<IResult> GetAllCustomers(ICustomerService customerService)
    {
        return await customerService.GetAllCustomers();
    }
    
    private async Task<IResult> GetCustomerByUserName(ICustomerService customerService, string userName)
    {
        var customer = await customerService.GetCustomerByUserNameAsync(userName);
        return  customer != null ? Results.Ok(customer) : Results.NotFound();
    }

    #endregion
}