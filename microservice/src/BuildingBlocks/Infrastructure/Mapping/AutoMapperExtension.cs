using AutoMapper;
using System.Reflection;

namespace Infrastructure.Mapping
{
    public static class AutoMapperExtension
    {
        // Extension method to skip a property which is not declare between source and destination, to skip a case when update a entity having forgein key
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceType = typeof(TSource);
            var destinationProps = typeof(TDestination).GetProperties(flags);

            foreach (var prop in destinationProps) {
                if (sourceType.GetProperty(prop.Name, flags) == null)
                    expression.ForMember(prop.Name, option => option.Ignore());
            }
            return expression;
        }
    }
}
