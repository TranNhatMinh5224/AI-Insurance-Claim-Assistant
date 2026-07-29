using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.Controllers.Requests;

public sealed record ResetPasswordRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Token,
    [Required] string NewPassword
);
