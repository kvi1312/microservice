using Inventory.API.Entities.Abstraction;
using Inventory.API.Extensions;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Inventory.API.Repositories.Abstraction;

public class MongoDbRepository<T> : IMongoDbRepositoryBase<T> where T : MongoEntity
{
    private IMongoDatabase _database { get; }

    // Get collection from where inherit this abstraction class
    protected virtual IMongoCollection<T> Collection => _database.GetCollection<T>(GetCollectionName());

    public MongoDbRepository(IMongoClient client, DatabaseSettings settings)
    {
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public Task CreateAsync(T entity) => Collection.InsertOneAsync(entity);

    public Task DeleteAsync(string id) => Collection.DeleteOneAsync(x => x.Id.Equals(id));

    public IMongoCollection<T> FindAll(ReadPreference? readPreference = null)
     => _database.WithReadPreference(readPreference ?? ReadPreference.Primary)
                .GetCollection<T>(GetCollectionName());

    public Task UpdateAsync(T entity)
    {
        Expression<Func<T, string>> func = f => f.Id;
        var value = (string)entity.GetType()
                                  .GetProperty(((MemberExpression)func.Body).Member.Name)? //func.Body.ToString().Split(".")[1]
                                  .GetValue(entity, null);
        var filter = Builders<T>.Filter.Eq(func, value);
        return Collection.ReplaceOneAsync(filter, entity);
    }

    private static string GetCollectionName()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), inherit: true)
                         .FirstOrDefault() as BsonCollectionAttribute)?.CollectionName ?? string.Empty;
    }
}
