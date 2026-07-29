using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Auth.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Security: Không trả về lỗi nếu email không tồn tại để tránh kẻ xấu dò quét email
        if (user is null)
        {
            return Result<bool>.Success(true);
        }

        // Tạo Token reset mật khẩu
        string token = user.GeneratePasswordResetToken();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Xây dựng Link (Frontend URL + token + email)
        // Ví dụ: https://frontend.com/reset-password?token=XYZ&email=abc@gmail.com
        string resetLink = $"{request.FrontendResetUrl.TrimEnd('/')}?token={token}&email={user.Email}";

        // Gửi email ở background (Fire and Forget) hoặc chờ await
        // Để đảm bảo tính nhất quán, ta dùng await
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink, user.FullName);

        return Result<bool>.Success(true);
    }
}
