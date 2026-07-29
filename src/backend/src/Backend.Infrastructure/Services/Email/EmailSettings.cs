namespace Backend.Infrastructure.Services.Email;

public sealed class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string AppPassword { get; set; } = string.Empty;
}
