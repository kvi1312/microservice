namespace Microservice.IDP.Infrastructure.Domain;

public interface IEntityBase<TKey>
{
    TKey Id { get; set; }
}