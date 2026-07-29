using Backend.Application.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Backend.Infrastructure.Services.Email;

internal sealed class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public SmtpEmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink, string fullName)
    {
        // 1. Tạo Message
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        message.To.Add(new MailboxAddress(fullName, toEmail));
        message.Subject = "Yêu cầu khôi phục mật khẩu - AI Insurance";

        // 2. Đọc Template từ file và thay thế biến
        string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "ResetPasswordTemplate.html");
        
        string htmlBody;
        if (File.Exists(templatePath))
        {
            htmlBody = await File.ReadAllTextAsync(templatePath);
            htmlBody = htmlBody.Replace("{{FullName}}", fullName)
                               .Replace("{{ResetLink}}", resetLink);
        }
        else
        {
            // Fallback nếu không thấy file template
            htmlBody = $"<h1>Xin chào {fullName},</h1><p>Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng bấm vào <a href='{resetLink}'>ĐÂY</a> để đổi mật khẩu. Link có hiệu lực 15 phút.</p>";
        }

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        // 3. Gửi qua SMTP bằng MailKit
        using var client = new SmtpClient();
        
        try
        {
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.AppPassword);
            await client.SendAsync(message);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
