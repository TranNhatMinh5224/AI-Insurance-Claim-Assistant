using System.Security.Claims;
using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Auth.RefreshToken;

internal sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Trích xuất Claims từ Access Token đã hết hạn
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
        {
            return Result<RefreshTokenResponse>.Failure(
                Error.Unauthorized("Token.Invalid", "Access Token không hợp lệ"));
        }

        var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Result<RefreshTokenResponse>.Failure(
                Error.Unauthorized("Token.InvalidPayload", "Access Token bị lỗi cấu trúc"));
        }

        // 2. Lấy User từ DB
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || user.RefreshToken != request.RefreshToken)
        {
            return Result<RefreshTokenResponse>.Failure(
                Error.Unauthorized("Token.InvalidRefresh", "Refresh Token không hợp lệ hoặc đã bị thay đổi"));
        }

        // 3. Kiểm tra hạn của Refresh Token
        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result<RefreshTokenResponse>.Failure(
                Error.Unauthorized("Token.ExpiredRefresh", "Refresh Token đã hết hạn. Vui lòng đăng nhập lại"));
        }

        // 4. Tạo cặp Token mới
        string newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        string newRefreshToken = _jwtTokenService.GenerateRefreshToken();
        
        user.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));

        // 5. Lưu vào DB
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Trả về kết quả (Manual Mapping)
        return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(
            UserId: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role.ToString(),
            AccessToken: newAccessToken,
            RefreshToken: newRefreshToken,
            ExpiresAt: DateTime.UtcNow.AddHours(1)
        ));
    }
}
