namespace api.Configurations
{
    public class SmtpSettings
    {
        public string Host { get; set; } = "sandbox.smtp.mailtrap.io";
        public int Port { get; set; } = 2525;
        public string Username { get; set; } = "676371114b67e7";
        public string Password { get; set; } = "bc8da23abb3d9f";
        public string FromAddress { get; set; } = "no_reply@example.com";
    }
}
