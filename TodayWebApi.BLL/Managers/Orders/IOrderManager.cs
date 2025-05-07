using TodayWebApi.BLL.Dtos.Orders;
using TodayWebAPi.DAL.Data.Entities;

namespace TodayWebApi.BLL.Managers.Orders
{
    public interface IOrderManager
    {
        Task<OrderToReturnDto> CreateOrderAsync(string buyerEmail, string deliveryMethodId, string basketId);
        Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(string buyerEmail);
        Task<OrderToReturnDto> GetOrderByIdAsync(string id, string buyerEmail);
        Task<OrderToReturnDto> GetOrderByIdForAdminAsync(string id);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
        Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus);
    }
}