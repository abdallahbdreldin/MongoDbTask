using TodayWebAPi.DAL.Data.Models;

namespace TodayWebApi.BLL.Managers
{
    public interface IPaymentManager
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);
    }
}
