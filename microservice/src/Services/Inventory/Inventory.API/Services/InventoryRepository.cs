using AutoMapper;
using Infrastructure.Common;
using Infrastructure.Common.Models;
using Infrastructure.Extensions;
using Inventory.API.Entities;
using Inventory.API.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Configurations;
using Shared.DTOS.Inventory;

namespace Inventory.API.Services
{
    public class InventoryRepository : MongoDbRepository<InventoryEntry>, IInventoryRepository
    {
        private readonly IMapper _mapper;
        public InventoryRepository(IMongoClient client, MongoDbSettings settings, IMapper mapper) : base(client, settings)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo)
        {
            var entities = await FindAll().Find(x => x.ItemNo.Equals(itemNo)).ToListAsync();
            var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
            return result;
        }

        public async Task<PageList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query)
        {
            var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
            var filterItemNo = Builders<InventoryEntry>.Filter.Eq(x => x.ItemNo, query.ItemNo());

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(x => x.DocumentNo, query.SearchTerm);
            }

            var andFilter = filterItemNo & filterSearchTerm;
            var pageList = await Collection.PaginatedListAsync(andFilter, pageIndex: query.PageIndex, pageSize: query.PageSize);
            var items = _mapper.Map<IEnumerable<InventoryEntryDto>>(pageList);
            var result = new PageList<InventoryEntryDto>(items, pageList.GetMetaData().TotalItems, pageIndex: query.PageIndex, pageSize: query.PageSize);

            return result;
        }

        public async Task<InventoryEntryDto> GetByIdAsync(string id)
        {
            FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(x => x.Id, id);
            var entity = await FindAll().Find(filter).FirstOrDefaultAsync();
            var result = _mapper.Map<InventoryEntryDto>(entity);
            return result;
        }

        public async Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto dto)
        {
            var entity = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                ItemNo = itemNo,
                Quantity = dto.Quantity,
                DocumentType = dto.DocumentType,
            };

            await CreateAsync(entity);
            var result = _mapper.Map<InventoryEntryDto>(entity);
            return result;
        }

        public async Task<InventoryEntryDto> SaveItemAsync(string itemNo, SalesProductDto dto)
        {
            var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                ItemNo = itemNo,
                ExternalDocumentNo = dto.ExternalDocumentNo,
                Quantity = dto.Quantity,
                DocumentType = dto.DocumentType,
            };
            await CreateAsync(itemToAdd);
            var result = _mapper.Map<InventoryEntryDto>(itemToAdd);
            return result;
        }

        public async Task DeleteByDocumentNoAsync(string documentNo)
        {
            FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(x => x.ItemNo, documentNo);
            await Collection.DeleteOneAsync(filter);
        }
    }
}
