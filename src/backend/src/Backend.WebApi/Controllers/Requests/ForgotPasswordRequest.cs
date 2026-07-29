using System.ComponentModel.DataAnnotations;

namespace Backend.WebApi.Controllers.Requests;

public sealed record ForgotPasswordRequest(
    [Required] [EmailAddress] string Email,
    [Required] string FrontendResetUrl
);
