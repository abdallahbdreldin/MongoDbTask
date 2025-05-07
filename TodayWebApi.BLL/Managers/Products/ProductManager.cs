using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using TodayWebApi.BLL.Dtos.Products;
using TodayWebAPi.DAL.Data.Entities;
using TodayWebAPi.DAL.Repos.UnitOfWork;

namespace TodayWebApi.BLL.Managers.Products
{
    public class ProductManager : IProductManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductManager> _logger;

        public ProductManager(IUnitOfWork unitOfWork, ILogger<ProductManager> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public ProductDto MapToDto(Product product)
        {
            if (product == null)
            {
                return null;
            }

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                PictureUrl = product.PictureUrl,
                InStock = product.InStock,
                BrandId = product.BrandId.ToString(),
                TypeId = product.TypeId.ToString()
            };
        }

        public async Task<(IReadOnlyList<ProductDto> Products, int TotalCount)> GetAllWithDetails(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching products with pagination: page {PageNumber}, size {PageSize}", pageNumber, pageSize);


            var (products, totalCount) = await _unitOfWork.ProductRepo.GetAllWithPaginationAsync(pageNumber, pageSize);

            var productDtos = products.Select(p => MapToDto(p)).ToList();

            return (productDtos, totalCount);
        }

        public async Task<ProductDto> GetProductWithDetailsAsync(string id)
        {
            _logger.LogInformation("Fetching product with ID {Id}", id);
            var product = await _unitOfWork.ProductRepo.GetByIdAsync(id);
            if (product == null)
                return null;

            return MapToDto(product);
        }

        public async Task Add(ProductDto productDto)
        {
            _logger.LogInformation("Adding product {Name}", productDto.Name);
            var product = new Product
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                PictureUrl = productDto.PictureUrl,
                InStock = productDto.InStock,
                BrandId = productDto.BrandId,
                TypeId = productDto.TypeId,
            };
            await _unitOfWork.ProductRepo.AddAsync(product);
        }

        public async Task Update(UpdateProductDto updateProductDto)
        {
            _logger.LogInformation("Updating product with ID {Id}", updateProductDto.Id);
            var product = await _unitOfWork.ProductRepo.GetByIdAsync(updateProductDto.Id);
            if (product == null)
                throw new Exception("Product not found");

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.Price = updateProductDto.Price;
            product.PictureUrl = updateProductDto.PictureUrl;
            product.InStock = updateProductDto.InStock;
            product.BrandId = updateProductDto.BrandId;
            product.TypeId = updateProductDto.TypeId;


            await _unitOfWork.ProductRepo.UpdateAsync(product);
        }

        public async Task Remove(string id)
        {
            var product = await _unitOfWork.ProductRepo.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            _logger.LogInformation("Deleting product with ID {Id}", id);
            await _unitOfWork.ProductRepo.DeleteAsync(id);

        }

        public async Task<IReadOnlyList<ProductDto>> SearchProducts(string keyword)
        {
            _logger.LogInformation("Searching products with keyword {Keyword}", keyword);
            var products = await _unitOfWork.ProductRepo.SearchProductsAsync(keyword);

            return products.Select(p => MapToDto(p)).ToList();
        }

        public async Task<(IReadOnlyList<ProductDto> Products, int TotalCount)> FilterProducts(
            string? category, decimal? minPrice, decimal? maxPrice, bool? inStock, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Filtering products: category={Category}, minPrice={MinPrice}, maxPrice={MaxPrice}, inStock={InStock}", category, minPrice, maxPrice, inStock);

            var (products, totalCount) = await _unitOfWork.ProductRepo.FilterProductsAsync(category, minPrice, maxPrice, inStock, pageNumber, pageSize);

            var productDtos = products.Select(p => MapToDto(p)).ToList();

            return (productDtos, totalCount);
        }
    }
}