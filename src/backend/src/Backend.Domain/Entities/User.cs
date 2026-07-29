using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public sealed class User
{
    private User() { } // Required for EF Core

    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiryTime { get; private set; }

    public static User Create(
        string fullName,
        string email,
        string passwordHash,
        string? phoneNumber = null,
        UserRole role = UserRole.Customer)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            PhoneNumber = phoneNumber?.Trim(),
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // Domain method — thay đổi mật khẩu và cập nhật timestamp
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    // Domain method — cập nhật Refresh Token
    public void UpdateRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        UpdatedAt = DateTime.UtcNow;
    }

    // Domain method — Tạo token quên mật khẩu
    public string GeneratePasswordResetToken()
    {
        // Sinh ra chuỗi Guid ngẫu nhiên không có dấu gạch ngang (32 ký tự)
        string token = Guid.NewGuid().ToString("N");
        PasswordResetToken = token;
        PasswordResetTokenExpiryTime = DateTime.UtcNow.AddMinutes(15); // Hạn 15 phút
        UpdatedAt = DateTime.UtcNow;
        return token;
    }

    // Domain method — Đặt lại mật khẩu (Dùng khi quên mật khẩu)
    public void ResetPassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        PasswordResetToken = null;
        PasswordResetTokenExpiryTime = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
