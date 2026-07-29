using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Auth.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword
) : IRequest<Result<bool>>;
