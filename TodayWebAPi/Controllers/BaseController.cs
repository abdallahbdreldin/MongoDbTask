using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TodayWebAPi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected string GetUserEmail()
        {
            return User?.Claims?.FirstOrDefault(e => e.Type == ClaimTypes.Email)?.Value;
        }
    }
}
