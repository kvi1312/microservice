using Infrastructure.Common.Models;
using MongoDB.Driver;

namespace Infrastructure.Extensions;

public static class MongoCollectionExtension
{
    public static Task<PageList<TDestination>> PaginatedListAsync<TDestination>(
        this IMongoCollection<TDestination> collection, FilterDefinition<TDestination> filter, int pageIndex, int pageSize)
        where TDestination : class
    => PageList<TDestination>.ToPageList(collection, filter, pageIndex, pageSize);
}
