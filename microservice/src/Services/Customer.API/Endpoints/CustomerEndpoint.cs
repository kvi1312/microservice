using Carter;
using Customer.API.Services.Interfaces;

namespace Customer.API.Endpoints;

public class CustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/customers", GetAllCustomers);
        app.MapGet("api/customers/{userName}", GetCustomerByUserName);
    }

    #region Delegate Funcs
    private static async Task<IResult> GetAllCustomers(ICustomerService customerService)
    {
        return await customerService.GetAllCustomers();
    }
    
    private static async Task<IResult> GetCustomerByUserName(ICustomerService customerService, string userName)
    {
        var customer = await customerService.GetCustomerByUserNameAsync(userName);
        return  customer != null ? Results.Ok(customer) : Results.NotFound();
    }

    #endregion
}