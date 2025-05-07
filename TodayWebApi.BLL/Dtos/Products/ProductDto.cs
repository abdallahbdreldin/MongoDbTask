namespace TodayWebApi.BLL.Dtos.Products
{
    public class ProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; }
        public int InStock { get; set; }
        public string BrandId { get; set; }
        public string TypeId { get; set; }
    }
}
