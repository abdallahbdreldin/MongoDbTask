namespace TodayWebApi.BLL.Dtos
{
    public class OrderToReturnDto
    {
        public string Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string DeliveryMethodId { get; set; }
        public decimal ShippingPrice { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string PaymentIntentId { get; set; }
        public string PaymentId { get; set; }
    }
}
