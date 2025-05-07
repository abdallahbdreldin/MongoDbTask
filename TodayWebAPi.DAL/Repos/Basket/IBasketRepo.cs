using TodayWebAPi.DAL.Data.Entities;

namespace TodayWebAPi.DAL.Repos.Basket
{
    public interface IBasketRepo
    {
        Task<CustomerBasket> GetBasketAsync(string basketId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string basketId);
    }
}
