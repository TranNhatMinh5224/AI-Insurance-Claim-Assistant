using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<Result<bool>>;
