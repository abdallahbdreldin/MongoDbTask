using TodayWebApi.BLL.Dtos.Products;

namespace TodayWebApi.BLL.Managers.Products
{
    public interface IProductManager
    {
        Task<(IReadOnlyList<ProductDto> Products, int TotalCount)> GetAllWithDetails(int pageNumber, int pageSize);
        Task<IReadOnlyList<ProductDto>> SearchProducts(string keyword);
        Task<(IReadOnlyList<ProductDto> Products, int TotalCount)> FilterProducts(string? category, decimal? minPrice, decimal? maxPrice, bool? inStock, int pageNumber, int pageSize);
        Task<ProductDto> GetProductWithDetailsAsync(string id);
        Task Add(ProductDto productDto);
        Task Update(UpdateProductDto updateProductDto);
        Task Remove(string id);
    }
}