namespace Ordering.Domain.Exceptions;

public class InvalidEntityTypeExceptions : ApplicationException
{
    public InvalidEntityTypeExceptions(string entity, object key) : base($"Entity \"{entity}\" not supported type {key}")
    {
        
    }
}