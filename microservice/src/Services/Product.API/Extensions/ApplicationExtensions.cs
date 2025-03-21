namespace Product.API.Extensions;

public static class ApplicationExtensions
{
    // This class handle used things for application
    public static void UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        // app.UseHttpsRedirection();

        app.UseRouting();
        app.UseAuthentication();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}