namespace TodayWebApi.BLL.Managers
{
    public interface IEmailManager
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
