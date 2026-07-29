using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Lấy thông tin user
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<bool>.Failure(Error.NotFound("User.NotFound", "Không tìm thấy người dùng"));

        // Step 2: Kiểm tra mật khẩu hiện tại có đúng không
        bool isCurrentPasswordValid = _passwordHasher.Verify(request.CurrentPassword, user.PasswordHash);
        if (!isCurrentPasswordValid)
            return Result<bool>.Failure(
                Error.Validation("User.InvalidPassword", "Mật khẩu hiện tại không chính xác"));

        // Step 3: Hash mật khẩu mới và gọi domain method
        string newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newPasswordHash); // ← Domain logic chạy ở đây

        // Step 4: Lưu vào DB
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
