using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber
) : IRequest<Result<RegisterResponse>>;
