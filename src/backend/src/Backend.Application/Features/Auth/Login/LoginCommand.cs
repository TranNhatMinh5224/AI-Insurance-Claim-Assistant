using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;
