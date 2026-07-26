
namespace GymX.Infrastructure.Options
{
    public class MailOptions
    {
        public const string SectionName = "Mail";
        public string From { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
