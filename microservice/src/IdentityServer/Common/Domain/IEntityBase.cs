namespace IdentityServer.Common.Domain;

public interface IEntityBase<TKey>
{
    TKey Id { get; set; }
}