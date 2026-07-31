namespace Backend.Application.Abstractions;

/// <summary>
/// Trích xuất thông tin người dùng hiện tại từ JWT Token.
/// Inject interface này vào Handler thay vì dùng IHttpContextAccessor trực tiếp.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>UserId lấy từ Claim "NameIdentifier" trong JWT. Null nếu chưa đăng nhập.</summary>
    Guid? UserId { get; }

    /// <summary>Trả về UserId hoặc throw nếu chưa đăng nhập (dùng trong các endpoint bắt buộc [Authorize]).</summary>
    Guid GetUserIdOrThrow();

    /// <summary>Role của người dùng hiện tại.</summary>
    string? Role { get; }

    bool IsAuthenticated { get; }
}
