using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string Email,
    string FrontendResetUrl
) : IRequest<Result<bool>>;
