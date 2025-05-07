namespace TodayWebApi.BLL.Dtos.Orders
{
    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
