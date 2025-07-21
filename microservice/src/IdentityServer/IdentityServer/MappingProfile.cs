using AutoMapper;
using Microservice.IDP.Infrastructure.Entities;
using Microservice.IDP.Infrastructure.ViewModels;
using Microservices.IDP.Infrastructure.ViewModels;

namespace IdentityServer;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Permission, PermissionViewModel>();
        CreateMap<Permission, PermissionUserViewModel>();
    }
}
