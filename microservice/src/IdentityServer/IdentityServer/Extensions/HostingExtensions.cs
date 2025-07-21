using AutoMapper;
using IdentityServer.Services.EmailServices;
using Microservice.IDP.Infrastructure.Domain;
using Microservice.IDP.Infrastructure.Repositories;
using Microservices.IDP.Presentation;
using Serilog;

namespace IdentityServer.Extensions;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddConfigurationSettings(builder.Configuration);
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Services.AddScoped<IEmailSender, SMTPEmailService>();
        builder.Services.ConfigureCookiePolicy();
        builder.Services.ConfigureCors();
        builder.Services.ConfigureIdentity(builder.Configuration);
        builder.Services.ConfigureIdentityServer(builder.Configuration);
        builder.Services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
        builder.Services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
        builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
        builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
        builder.Services.AddControllers(config =>
        {
            config.RespectBrowserAcceptHeader = true;
            config.ReturnHttpNotAcceptable = true;
        }).AddApplicationPart(typeof(AssemblyReference).Assembly);

        builder.Services.ConfigureAuthentication();
        builder.Services.ConfigureAuthorization();

        builder.Services.ConfigureSwagger(builder.Configuration);
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseCors("CorsPolicy");
        app.UseSwagger();
        app.UseSwaggerUI(c => {c.OAuthClientId("microservices_swagger");  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API"); c.DisplayRequestDuration(); });
        app.UseRouting();
        app.UseMiddleware<ErrorWrappingMiddleware>();
        //set cookie before authentication and authorization
        app.UseCookiePolicy();
        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute().RequireAuthorization("Bearer");
            endpoints.MapRazorPages().RequireAuthorization();
        });

        return app;
    }
}