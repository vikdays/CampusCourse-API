namespace api.Services.Interfaces
{
    public interface IEmailSender
    {
        public Task SendEmail(string toEmail, string subject, string body);
    }
}
