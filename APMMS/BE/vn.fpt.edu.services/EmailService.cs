using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtp = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(
                _config["Email:Username"],
                _config["Email:Password"]),
            EnableSsl = true,
        };

        var message = new MailMessage(_config["Email:Username"], to, subject, body)
        {
            IsBodyHtml = true
        };

        await smtp.SendMailAsync(message);
    }
}
