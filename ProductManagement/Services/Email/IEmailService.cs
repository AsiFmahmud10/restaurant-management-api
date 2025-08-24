namespace ProductManagement.Services.Email;

public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}