using TodayWebAPi.DAL.Data.Models;

namespace TodayWebApi.BLL.Managers.Shared.Payment
{
    public interface IPaymentManager
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);
    }
}
