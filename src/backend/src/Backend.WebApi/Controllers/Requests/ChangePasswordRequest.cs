namespace Backend.WebApi.Controllers.Requests;

// Request này dùng để bind từ JSON body, không chứa UserId vì UserId sẽ được lấy từ Token
public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
);
