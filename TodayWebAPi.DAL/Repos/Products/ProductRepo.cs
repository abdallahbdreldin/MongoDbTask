using MongoDB.Bson;
using MongoDB.Driver;
using TodayWebAPi.DAL.Data.Context;
using TodayWebAPi.DAL.Data.Entities;
using TodayWebAPi.DAL.Repos.Generic;

namespace TodayWebAPi.DAL.Repos.Products
{
    public class ProductRepo : GenericRepo<Product> , IProductRepo
    {
        private readonly StoreContext _context;

        public ProductRepo(StoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(IReadOnlyList<Product>, int)> GetAllWithPaginationAsync(int pageNumber, int pageSize)
        {
            var products = await _context.GetCollection<Product>()
                .Find(p => true)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            int totalCount = (int)await _context.GetCollection<Product>().CountDocumentsAsync(p => true);

            return (products, totalCount);
        }

        public async Task<IReadOnlyList<Product>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await _context.GetCollection<Product>().Find(p => true).ToListAsync();

            var filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(keyword, "i"));
            return await _context.GetCollection<Product>().Find(filter).ToListAsync();
        }

        public async Task<(IReadOnlyList<Product>, int)> FilterProductsAsync(string category, decimal? minPrice, decimal? maxPrice, bool? inStock, int pageNumber, int pageSize)
        {
            var filter = Builders<Product>.Filter.Empty;

            if (!string.IsNullOrEmpty(category))
            {
                var type = _context.GetCollection<ProductType>()
                                .Find(t => t.Name == category)
                                .FirstOrDefault();

                filter &= Builders<Product>.Filter.Eq(p => p.TypeId, type.Id);
            }

            if (minPrice.HasValue)
            {
                filter &= Builders<Product>.Filter.Gte(p => p.Price, minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                filter &= Builders<Product>.Filter.Lte(p => p.Price, maxPrice.Value);
            }

            if (inStock.HasValue)
            {
                if (inStock.Value)
                {
                    filter &= Builders<Product>.Filter.Gt(p => p.InStock, 0);
                }
                else
                {
                    filter &= Builders<Product>.Filter.Eq(p => p.InStock, 0);
                }
            }

            var products = await _context.GetCollection<Product>()
                .Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            int totalCount = (int)await _context.GetCollection<Product>().CountDocumentsAsync(filter);

            return (products, totalCount);
        }
    }
}