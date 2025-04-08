using System.Reflection;
using AutoMapper;

namespace Ordering.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    // Auto calling Mapping(Profile profile) when a class in assembly implement IMapFrom<T>
    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var mappingInterfaceType = typeof(IMapFrom<>);
        const string mappingMethodName = nameof(IMapFrom<object>.Mapping);

        var typesWithMapping = assembly.GetExportedTypes()
                                        .Where(type => type.GetInterfaces()
                                        .Any(i => IsMappingInterface(i, mappingInterfaceType)))
                                        .ToList();

        foreach (var type in typesWithMapping)
        {
            var instance = Activator.CreateInstance(type);
            if (instance == null) continue;

            // Prefer calling mapping method from class
            var mappingMethod = type.GetMethod(mappingMethodName);
            if (mappingMethod != null)
            {
                mappingMethod.Invoke(instance, new object[] { this });
                continue;
            }

            // otherwise, calling mapping method from interface
            foreach (var interfaceType in type.GetInterfaces()
                         .Where(i => IsMappingInterface(i, mappingInterfaceType)))
            {
                var interfaceMappingMethod = interfaceType.GetMethod(mappingMethodName, new[] { typeof(Profile) });
                interfaceMappingMethod?.Invoke(instance, new object[] { this });
            }
        }
    }

    private static bool IsMappingInterface(Type interfaceType, Type mappingInterfaceType)
    {
        return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == mappingInterfaceType;
    }
}