using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<bool>.Failure(Error.Unauthorized("Auth.Unauthenticated", "Không xác định được người dùng."));

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
            return Result<bool>.Failure(Error.NotFound("User.NotFound", "Không tìm thấy người dùng"));

        bool isCurrentPasswordValid = _passwordHasher.Verify(request.CurrentPassword, user.PasswordHash);
        if (!isCurrentPasswordValid)
            return Result<bool>.Failure(
                Error.Validation("User.InvalidPassword", "Mật khẩu hiện tại không chính xác"));

        string newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newPasswordHash);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
