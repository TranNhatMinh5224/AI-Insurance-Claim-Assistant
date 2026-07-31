using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Users.GetProfile;

internal sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<GetUserProfileResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public GetUserProfileQueryHandler(IUserRepository userRepository, ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<GetUserProfileResponse>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<GetUserProfileResponse>.Failure(
                Error.Unauthorized("Auth.Unauthenticated", "Không xác định được người dùng."));

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
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
