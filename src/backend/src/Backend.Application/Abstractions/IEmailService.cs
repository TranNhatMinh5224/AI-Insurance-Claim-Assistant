namespace Backend.Application.Abstractions;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink, string fullName);
}
