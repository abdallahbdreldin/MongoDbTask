using TodayWebAPi.DAL.Data.Identity;

namespace TodayWebApi.BLL.Managers.Shared.Token
{
    public interface ITokenManager
    {
        Task<string> CreateToken(User user);
    }
}
