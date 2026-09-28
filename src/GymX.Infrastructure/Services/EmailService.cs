using GymX.Application.Common.Interfaces;
using GymX.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GymX.Infrastructure.Services;

public class EmailService(
    IOptions<MailOptions> mailOptions,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly MailOptions _options = mailOptions.Value;

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_options.DisplayName, _options.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Timeout = 10000; 
            
            await smtp.ConnectAsync(_options.SmtpServer, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            
            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Đã gửi email thành công tới {To}", to);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi gửi email tới {To}. Chi tiết: {Message}", to, ex.Message);
            return false;
        }
    }
}
