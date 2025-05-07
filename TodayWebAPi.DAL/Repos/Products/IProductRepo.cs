using TodayWebAPi.DAL.Data.Entities;
using TodayWebAPi.DAL.Repos.Generic;

namespace TodayWebAPi.DAL.Repos.Products
{
    public interface IProductRepo : IGenericRepo<Product>
    {
        Task<(IReadOnlyList<Product>, int)> GetAllWithPaginationAsync(int pageNumber, int pageSize);
        Task<(IReadOnlyList<Product>, int)> FilterProductsAsync(string category, decimal? minPrice, decimal? maxPrice, bool? inStock, int pageNumber, int pageSize);
        Task<IReadOnlyList<Product>> SearchProductsAsync(string keyword);
    }
}
