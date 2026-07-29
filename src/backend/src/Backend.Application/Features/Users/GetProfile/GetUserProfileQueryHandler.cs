using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Users.GetProfile;

internal sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<GetUserProfileResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetUserProfileResponse>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<GetUserProfileResponse>.Failure(
                Error.NotFound("User.NotFound", "Không tìm thấy thông tin người dùng"));

        return Result<GetUserProfileResponse>.Success(new GetUserProfileResponse(
            UserId: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            PhoneNumber: user.PhoneNumber,
            Role: user.Role.ToString(),
            CreatedAt: user.CreatedAt,
            UpdatedAt: user.UpdatedAt
        ));
    }
}
