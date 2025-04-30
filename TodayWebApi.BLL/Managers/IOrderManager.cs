using TodayWebApi.BLL.Dtos;
using TodayWebAPi.DAL.Data.Models;

namespace TodayWebApi.BLL.Managers
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