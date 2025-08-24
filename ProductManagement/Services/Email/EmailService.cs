using System.Net.Mail;
using Microsoft.Extensions.Options;
using ProductManagement.Config;

namespace ProductManagement.Services.Email;

public class EmailService(IOptions<EmailSettings> emailSettingsOption) : IEmailService
{
    private EmailSettings EmailSettings { get; } = emailSettingsOption.Value;
    
    public void SendEmail(string to, string subject, string body)
    {
        SmtpClient  client = new SmtpClient(EmailSettings.Host,EmailSettings.Port);
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        client.Credentials = new System.Net.NetworkCredential(EmailSettings.Username,EmailSettings.Password);
        MailMessage message = new MailMessage(EmailSettings.Username, to, subject, body); 
        message.IsBodyHtml = true;
        
        
        client.Send(message);
    }
}

