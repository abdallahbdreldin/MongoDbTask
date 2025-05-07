namespace TodayWebApi.BLL.Managers.Shared.Email
{
    public interface IEmailManager
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
