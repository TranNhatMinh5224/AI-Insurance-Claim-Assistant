using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Auth.Login;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        const string invalidCredentialsMessage = "Email hoặc mật khẩu không chính xác";

        // Step 1: Tìm user theo email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Result<LoginResponse>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", invalidCredentialsMessage));

        // Step 2: Xác thực password
        bool isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            return Result<LoginResponse>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", invalidCredentialsMessage));

        // Step 3: Tạo JWT Access Token & Refresh Token
        string accessToken = _jwtTokenService.GenerateAccessToken(user);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();
        
        // Thời hạn Refresh Token thường dài hơn (VD: 7 ngày)
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); 
        user.UpdateRefreshToken(refreshToken, refreshTokenExpiry);

        // Lưu vào DB
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 4: Trả về kết quả
        return Result<LoginResponse>.Success(new LoginResponse(
            UserId: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role.ToString(),
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: DateTime.UtcNow.AddHours(1)
        ));
    }
}
