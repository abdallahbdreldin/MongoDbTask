using TodayWebAPi.DAL.Data.Identity;

namespace TodayWebApi.BLL.Managers
{
    public interface ITokenManager
    {
        Task<string> CreateToken(User user);
    }
}
