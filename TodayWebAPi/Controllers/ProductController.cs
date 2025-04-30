using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodayWebApi.BLL.Dtos;
using TodayWebApi.BLL.Managers;

namespace TodayWebAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductManager _productManager;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductManager productManager, ILogger<ProductController> logger)
        {
            _productManager = productManager ?? throw new ArgumentNullException(nameof(productManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllProducts([FromQuery] PaginationDto paginationParams)
        {
            try
            {
                _logger.LogInformation("Fetching all products: page {PageNumber}, size {PageSize}",
                    paginationParams.PageNumber, paginationParams.PageSize);

                var (products, totalCount) = await _productManager.GetAllWithDetails(
                    paginationParams.PageNumber, paginationParams.PageSize);

                var response = new
                {
                    Data = products,
                    TotalCount = totalCount,
                    PageNumber = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all products");
                return StatusCode(500, "An error occurred while fetching products.");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> Search([FromQuery] string keyword)
        {
            try
            {
                _logger.LogInformation("Searching products with keyword: {Keyword}", keyword);
                var products = await _productManager.SearchProducts(keyword);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with keyword: {Keyword}", keyword);
                return StatusCode(500, "An error occurred while searching products.");
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(
            [FromQuery] string? category,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? inStock,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Filtering products: category={Category}, minPrice={MinPrice}, maxPrice={MaxPrice}, inStock={InStock}, page={PageNumber}, size={PageSize}",
                    category, minPrice, maxPrice, inStock, pageNumber, pageSize);

                var (products, totalCount) = await _productManager.FilterProducts(
                    category, minPrice, maxPrice, inStock, pageNumber, pageSize);

                var response = new
                {
                    Data = products,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering products");
                return StatusCode(500, "An error occurred while filtering products.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(string id)
        {
            try
            {
                _logger.LogInformation("Fetching product with ID: {Id}", id);
                var product = await _productManager.GetProductWithDetailsAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found", id);
                    return NotFound("Product not found");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product with ID: {Id}", id);
                return StatusCode(500, "An error occurred while fetching the product.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] ProductDto productDto)
        {
            try
            {
                if (productDto == null)
                {
                    _logger.LogWarning("Invalid product data received for AddProduct");
                    return BadRequest("Invalid product data");
                }

                _logger.LogInformation("Adding product: {Name}", productDto.Name);
                await _productManager.Add(productDto);
                return CreatedAtAction(nameof(GetProduct), new { id = productDto.Id }, productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product: {Name}", productDto?.Name);
                return StatusCode(500, "An error occurred while adding the product.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                if (updateProductDto == null || id != updateProductDto.Id)
                {
                    _logger.LogWarning("Invalid product data or ID mismatch for UpdateProduct: {Id}", id);
                    return BadRequest("Invalid product data or ID mismatch");
                }

                _logger.LogInformation("Updating product with ID: {Id}", id);
                await _productManager.Update(updateProductDto);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {Id}", id);
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            try
            {
                await _productManager.Remove(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}