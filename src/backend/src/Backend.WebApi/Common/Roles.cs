namespace Backend.WebApi.Common;

/// <summary>
/// Hằng số tên các Role — dùng trong [Authorize(Roles = Roles.SuperAdmin)].
/// Luôn sử dụng class này thay vì string literal để tránh lỗi typo.
/// </summary>
public static class Roles
{
    public const string Customer = "Customer";
    public const string ClaimOfficer = "ClaimOfficer";
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>Dùng khi endpoint cho phép cả ClaimOfficer và SuperAdmin.</summary>
    public const string StaffAndAdmin = $"{ClaimOfficer},{SuperAdmin}";

    /// <summary>Dùng khi endpoint yêu cầu đăng nhập nhưng không giới hạn Role.</summary>
    public const string Any = $"{Customer},{ClaimOfficer},{SuperAdmin}";
}
