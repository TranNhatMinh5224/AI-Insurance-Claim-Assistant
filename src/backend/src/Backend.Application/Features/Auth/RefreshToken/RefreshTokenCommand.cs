using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : IRequest<Result<RefreshTokenResponse>>;
