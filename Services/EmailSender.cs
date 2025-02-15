using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        using var client = new SmtpClient();
        var credentials = new NetworkCredential(
            _configuration["Email:SmtpUser"],
            _configuration["Email:SmtpPassword"]
        );

        client.Credentials = credentials;
        client.Host = _configuration["Email:SmtpHost"];
        client.Port = int.Parse(_configuration["Email:SmtpPort"]);
        client.EnableSsl = true;

        var mailMessage = new MailMessage(
            _configuration["Email:FromEmail"],
            email,
            subject,
            message
        );
        mailMessage.IsBodyHtml = true;

        await client.SendMailAsync(mailMessage);
    }
} 