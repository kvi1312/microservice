namespace Microservice.IDP.Infrastructure.Domain;

public interface IEntityBase<T>
{
    T Id { get; set; }
}