using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using MediatR;

namespace Backend.Application.Features.Auth.Register;

internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Check if email already exists
        bool emailExists = await _userRepository.IsEmailExistsAsync(request.Email, cancellationToken);
        if (emailExists)
        {
            return Result<RegisterResponse>.Failure(
                Error.Conflict("User.EmailAlreadyExists", $"Email '{request.Email}' đã được sử dụng"));
        }

        // Step 2: Hash password
        string passwordHash = _passwordHasher.Hash(request.Password);

        // Step 3: Create User entity via factory method
        var user = User.Create(
            fullName: request.FullName,
            email: request.Email,
            passwordHash: passwordHash,
            phoneNumber: request.PhoneNumber);

        // Step 4: Persist
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 5: Return success response
        return Result<RegisterResponse>.Success(
            new RegisterResponse(user.Id, user.Email, user.FullName));
    }
}
