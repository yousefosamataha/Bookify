using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Bookify.Web.Services;

public class EmailSender : IEmailSender
{
    private readonly MailSettings _mailSettings;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EmailSender(IOptions<MailSettings> mailSettings, IWebHostEnvironment webHostEnvironment)
    {
        _mailSettings = mailSettings.Value;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        MailMessage message = new ()
        {
            From = new MailAddress(_mailSettings.Email!, _mailSettings.DisplayName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        message.To.Add(_webHostEnvironment.IsDevelopment() ? "yousefosamataha14122002@gmail.com" : email);

        SmtpClient smtpClient = new(_mailSettings.Host)
        {
            Port = _mailSettings.Port,
            Credentials = new NetworkCredential(_mailSettings.Email, _mailSettings.Password),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(message);
        smtpClient.Dispose();
    }
}
