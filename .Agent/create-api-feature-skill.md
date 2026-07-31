---
skill_name: "Create New API Feature - Clean Architecture"
version: "1.1"
changelog: "v1.1 — Thêm RULE A4: Phân biệt HTTP Status Code theo Error.Code cho Auth endpoints"
tech_stack: ".NET 10, C# 14, MediatR, EF Core 10 (PostgreSQL), FluentValidation, BCrypt, JWT Bearer"
project: "AI-Insurance-Claim-Assistant / Backend"
roles: "Customer, ClaimOfficer (no Manager/Admin in MVP)"
applies_to: "Backend.Domain, Backend.Application, Backend.Infrastructure, Backend.WebApi"
last_updated: "2026-07-28"
---

# Agent Skill: Tạo API Feature Mới (Clean Architecture)

## 1. CONTEXT — Khi nào kích hoạt Skill này

### ✅ Kích hoạt khi:
- Yêu cầu TẠO MỚI một API endpoint cho backend C#
- Yêu cầu THÊM tính năng nghiệp vụ mới vào hệ thống bảo hiểm
- Yêu cầu REFACTOR một API cũ theo chuẩn Clean Architecture

### ❌ KHÔNG áp dụng khi:
- Yêu cầu liên quan đến Python/FastAPI AI Service
- Yêu cầu sửa bug nhỏ không ảnh hưởng kiến trúc
- Yêu cầu chỉ về Frontend

---

## 2. ARCHITECTURE MAP — Bản đồ Kiến trúc

### Sơ đồ phụ thuộc (Dependency Direction):
```
Backend.WebApi
    └── depends on → Backend.Application
                         └── depends on → Backend.Domain
Backend.Infrastructure
    └── depends on → Backend.Application
                         └── depends on → Backend.Domain
```

**LUẬT VÀNG:** Mũi tên chỉ đi vào trong (vào Domain). KHÔNG BAO GIỜ ngược lại.

### Luồng HTTP Request:
```
HTTP Request
    → AuthController (WebApi)
        → ISender.Send(RegisterCommand)  [MediatR]
            → ValidationPipelineBehavior  [FluentValidation auto-run]
                → RegisterCommandHandler (Application)
                    → IUserRepository.IsEmailExistsAsync()
                    → IPasswordHasher.Hash()
                    → IUserRepository.AddAsync()
                    → IUnitOfWork.SaveChangesAsync()
                        → UserRepository (Infrastructure)
                            → AppDbContext (EF Core + PostgreSQL)
    ← Result<RegisterResponse>
    ← ApiResponse<RegisterResponse> (JSON chuẩn)
```

### Trách nhiệm từng Layer:

#### Backend.Domain
- ✅ Entities: `User.cs`, các entity khác
- ✅ Enums: `UserRole.cs` (Customer, ClaimOfficer)
- ✅ Common: `Result<T>.cs`, `Error.cs`
- ❌ KHÔNG import EF Core, MediatR, hay bất kỳ package external nào

#### Backend.Application
- ✅ Features (CQRS): Commands, Queries, Handlers, Validators
- ✅ Abstractions (Interfaces): `IUserRepository`, `IPasswordHasher`, `IUnitOfWork`
- ✅ Behaviors: `ValidationPipelineBehavior`
- ❌ KHÔNG import EF Core, Npgsql, BCrypt, hoặc Infrastructure

#### Backend.Infrastructure
- ✅ Persistence: `AppDbContext`, EF Configurations, Migrations
- ✅ Repositories: Implement IUserRepository, etc.
- ✅ Services: Implement IPasswordHasher (BCrypt), IJwtTokenService
- ❌ KHÔNG chứa business logic

#### Backend.WebApi
- ✅ Controllers: Chỉ nhận request → gọi MediatR → trả response
- ✅ Common: `ApiResponse<T>` wrapper
- ✅ Middlewares: `GlobalExceptionHandlerMiddleware`
- ✅ Program.cs: DI registration, middleware pipeline
- ❌ KHÔNG chứa business logic, KHÔNG gọi Repository trực tiếp

---

## 3. RULES — Quy tắc Bắt buộc

### GROUP A: Controller Rules

#### RULE A1: Controller PHẢI siêu mỏng (Thin Controller)
```csharp
// ✅ ĐÚNG — Controller chỉ làm 3 việc
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
{
    var result = await _sender.Send(command, ct);
    return result.IsSuccess
        ? Ok(ApiResponse<RegisterResponse>.Success("Đăng ký thành công", result.Value))
        : BadRequest(ApiResponse<RegisterResponse>.Failure(result.Error.Message));
}

// ❌ SAI — Controller chứa logic
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto dto)
{
    if (await _dbContext.Users.AnyAsync(u => u.Email == dto.Email)) // ← KHÔNG ĐƯỢC
        return BadRequest("Email đã tồn tại");
    // ...
}
```

#### RULE A2: Constructor chỉ Inject `ISender`
```csharp
// ✅ ĐÚNG
public AuthController(ISender sender) => _sender = sender;

// ❌ SAI — Inject service cụ thể
public AuthController(IUserRepository repo, IPasswordHasher hasher) { ... }
```

#### RULE A3: KHÔNG có try/catch trong Controller
Mọi exception được xử lý bởi `GlobalExceptionHandlerMiddleware`.

#### RULE A4: Phân biệt HTTP Status Code theo `Error.Code` — BẮT BUỘC với Auth endpoints

Các endpoint Auth (Login, đổi mật khẩu...) có thể gặp 2 loại lỗi khác nhau cần trả về đúng Status Code:

| Nguồn lỗi | `Error.Code` bắt đầu bằng | HTTP Status |
|---|---|---|
| FluentValidation (input rỗng/sai format) | `"Validation."` | `400 Bad Request` |
| Sai thông tin đăng nhập | `"Auth."` | `401 Unauthorized` |
| Không tìm thấy resource | `"NotFound."` | `404 Not Found` |
| Xung đột dữ liệu | `"Conflict."` / `"User."` | `409 Conflict` |

```csharp
// ✅ ĐÚNG — Phân biệt lỗi theo Error.Code
if (result.IsFailure)
{
    return result.Error.Code.StartsWith("Validation")
        ? BadRequest(ApiResponse<LoginResponse>.FailureResult(result.Error.Message))
        : Unauthorized(ApiResponse<LoginResponse>.FailureResult(result.Error.Message));
}

// ❌ SAI — Gộp tất cả lỗi vào 401 (sai với Validation errors)
if (result.IsFailure)
    return Unauthorized(ApiResponse<LoginResponse>.FailureResult(result.Error.Message));
```

**Nguồn gốc rule này:** Phát hiện khi review `AuthController.Login()` — Validation failure (email rỗng)
bị trả về 401 thay vì 400, làm sai ngữ nghĩa HTTP và gây khó debug cho Frontend.

---

### GROUP B: CQRS Rules

#### RULE B1: Phân biệt Command vs Query
- **Command** = thay đổi data: `CreateClaimCommand`, `RegisterCommand`, `UpdateClaimStatusCommand`
- **Query** = chỉ đọc data: `GetClaimByIdQuery`, `GetAllClaimsQuery`

#### RULE B2: Naming Convention BẮT BUỘC
```
Feature: Register
├── RegisterCommand.cs              ← [Verb][Feature]Command
├── RegisterCommandHandler.cs       ← [Verb][Feature]CommandHandler
├── RegisterCommandValidator.cs     ← [Verb][Feature]CommandValidator
└── RegisterResponse.cs             ← [Feature]Response
```

#### RULE B3: Handler LUÔN trả về `Result<T>`
```csharp
// ✅ ĐÚNG
public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken ct)
{
    if (await _userRepo.IsEmailExistsAsync(request.Email, ct))
        return Result<RegisterResponse>.Failure(Error.Conflict("User.EmailExists", "Email đã được sử dụng"));

    // ... logic
    return Result<RegisterResponse>.Success(new RegisterResponse(user.Id));
}

// ❌ SAI — Throw exception để điều hướng logic
if (emailExists) throw new Exception("Email exists"); // KHÔNG ĐƯỢC
```

---

### GROUP C: Validation Rules

#### RULE C1: Dùng FluentValidation, đặt trong Application Layer
```csharp
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Phải có ít nhất 1 chữ hoa")
            .Matches("[0-9]").WithMessage("Phải có ít nhất 1 chữ số");
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
    }
}
```

#### RULE C2: Controller KHÔNG ĐƯỢC kiểm tra ModelState
`ValidationPipelineBehavior` tự động chạy Validator trước Handler. Controller không cần làm gì thêm.

---

### GROUP D: Error Handling Rules

#### RULE D1: HTTP Status Code mapping chuẩn
| Tình huống | Status Code |
|---|---|
| Thành công (tạo mới) | `201 Created` |
| Thành công (truy vấn, cập nhật) | `200 OK` |
| Lỗi validation đầu vào | `400 Bad Request` |
| Chưa đăng nhập | `401 Unauthorized` |
| Không có quyền | `403 Forbidden` |
| Không tìm thấy | `404 Not Found` |
| Xung đột dữ liệu (email trùng) | `409 Conflict` |
| Lỗi hệ thống | `500 Internal Server Error` |

#### RULE D2: Response JSON chuẩn hóa (ApiResponse<T>)
```json
// Thành công
{ "success": true, "message": "Đăng ký thành công", "data": { "userId": "..." }, "errors": null }

// Thất bại
{ "success": false, "message": "Validation failed", "data": null, "errors": ["Email không hợp lệ"] }
```

---

### GROUP E: Naming & Structure Rules

#### RULE E1: Folder Structure bắt buộc
```
Backend.Application/
├── Abstractions/
│   ├── IUserRepository.cs
│   ├── IPasswordHasher.cs
│   └── IUnitOfWork.cs
├── Behaviors/
│   └── ValidationPipelineBehavior.cs
└── Features/
    └── Auth/
        └── Register/
            ├── RegisterCommand.cs
            ├── RegisterCommandHandler.cs
            ├── RegisterCommandValidator.cs
            └── RegisterResponse.cs

Backend.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   └── Configurations/
│       └── UserConfiguration.cs
├── Repositories/
│   └── UserRepository.cs
└── Services/
    └── PasswordHasherService.cs

Backend.WebApi/
├── Controllers/
│   └── AuthController.cs
├── Common/
│   └── ApiResponse.cs
└── Middlewares/
    └── GlobalExceptionHandlerMiddleware.cs
```

#### RULE E2: Domain Entities dùng private set
```csharp
// ✅ ĐÚNG — Không thể set từ bên ngoài
public string Email { get; private set; }

// ❌ SAI — Ai cũng set được
public string Email { get; set; }
```

---

### GROUP F: Database Rules (Project-Specific)

#### RULE F1: Database là PostgreSQL, dùng EF Core 10 + Npgsql
#### RULE F2: Dùng Guid cho Primary Key
#### RULE F3: LUÔN có `CreatedAt` và `UpdatedAt` trên mọi Entity
#### RULE F4: Roles hợp lệ trong MVP: `Customer`, `ClaimOfficer` (không có Admin/Manager)

### GROUP G: DTOs & Manual Mapping Rules

#### RULE G1: Sử dụng DTO phân tách rõ ràng
- **Input DTO (Web API):** Các class Request nhận từ client (VD: `ChangePasswordRequest`).
- **CQRS DTO (Application):** Command/Query và Response tương ứng (VD: `ChangePasswordCommand`, `GetUserProfileResponse`).
- **Entity (Domain):** Class chứa logic nghiệp vụ (VD: `User`).

#### RULE G2: BẮT BUỘC dùng Manual Mapping (Gán tay) — KHÔNG dùng AutoMapper
- Sử dụng Constructor hoặc Object Initializer để map dữ liệu.
- Giúp tận dụng tính năng kiểm tra kiểu của C# (Compile-time safety) và dễ dàng Find All References.
- **Anti-pattern:** Tuyệt đối không dùng AutoMapper trong CQRS vì làm ẩn logic, khó debug và giảm performance.

```csharp
// ✅ ĐÚNG: Gán tay thủ công (Manual Mapping)
var response = new GetUserProfileResponse(
    UserId: user.Id,
    FullName: user.FullName,
    Email: user.Email
);

// ❌ SAI: Dùng AutoMapper
var response = _mapper.Map<GetUserProfileResponse>(user);
```

---

### GROUP H: Current User Rules

#### RULE H1: BẮT BUỘC dùng `ICurrentUserService` để lấy UserId — KHÔNG dùng `IHttpContextAccessor` trực tiếp trong Handler

```csharp
// ✅ ĐÚNG — Inject ICurrentUserService
internal sealed class CreateCarCommandHandler : IRequestHandler<...>
{
    private readonly ICurrentUserService _currentUser;

    public async Task<Result<...>> Handle(...)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<...>.Failure(Error.Unauthorized("Auth.Unauthenticated", "..."));
    }
}

// ❌ SAI — Copy-paste ClaimsPrincipal lặp lại mọi Handler
var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId)) { ... } // Lặp lại!
```

**Nguyen tắc:** `ICurrentUserService` là abstraction duy nhất cho việc đọc JWT trong Application Layer.
Chỉ có Implementation `CurrentUserService.cs` ở Infrastructure mới được phép dùng `IHttpContextAccessor`.

#### RULE H2: `ICurrentUserService` có 3 thuộc tính cần biết
| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| `UserId` | `Guid?` | Null nếu chưa đăng nhập |
| `GetUserIdOrThrow()` | `Guid` | Throw `UnauthorizedAccessException` nếu null |
| `Role` | `string?` | Role của user (`Customer`, `ClaimOfficer`) |
| `IsAuthenticated` | `bool` | Kiểm tra trạng thái xem xác thực chưa |

---

### GROUP I: C# Code Style Rules

#### RULE I1: BẮT BUỘC dùng `using` thay vì Fully-Qualified Name

```csharp
// ✅ ĐÚNG — Khai báo using ở đầu file
using Microsoft.OpenApi.Models;

// ... sử dụng ngắn gọn
new OpenApiSecurityScheme { ... };

// ❌ SAI — Viết tên đầy đủ lặp lại nhiều lần
new Microsoft.OpenApi.Models.OpenApiSecurityScheme { ... };
```

**Quy tắc:** Mọi namespace đều phải được khai báo bằng `using` ở đầu file.
Tuyệt đối không viết inline fully-qualified name trong body của một method hay expression.

Luôn xây dựng từ trong ra ngoài (Domain → Application → Infrastructure → WebApi):

```
Bước 1: Domain Layer
  → Tạo/cập nhật Entity
  → Tạo Enum nếu cần

Bước 2: Application Layer
  → [a] Tạo Repository/Service Interface
  → [b] Tạo Command hoặc Query + Response DTO
  → [c] Tạo Validator
  → [d] Tạo Handler

Bước 3: Infrastructure Layer
  → [a] Cập nhật AppDbContext (thêm DbSet nếu cần)
  → [b] Tạo Entity Configuration (EF Fluent API)
  → [c] Implement Repository/Service
  → [d] Chạy Migration

Bước 4: WebApi Layer
  → [a] Thêm endpoint vào Controller (hoặc tạo Controller mới)
  → [b] Đăng ký DI nếu có service mới
```

---

## 5. CODE TEMPLATES

### Template 1: Entity (Domain Layer)
```csharp
namespace Backend.Domain.Entities;

public sealed class User
{
    private User() { } // EF Core constructor

    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static User Create(string fullName, string email, string passwordHash,
        string? phoneNumber, UserRole role = UserRole.Customer)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            PhoneNumber = phoneNumber,
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
```

### Template 2: Command + Handler (Application Layer)
```csharp
// RegisterCommand.cs
public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber
) : IRequest<Result<RegisterResponse>>;

// RegisterCommandHandler.cs
internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IUserRepository userRepository,
        IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await _userRepository.IsEmailExistsAsync(request.Email, ct))
            return Result<RegisterResponse>.Failure(
                Error.Conflict("User.EmailAlreadyExists", "Email đã được sử dụng"));

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.FullName, request.Email, passwordHash, request.PhoneNumber);

        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<RegisterResponse>.Success(new RegisterResponse(user.Id, user.Email));
    }
}
```

### Template 3: Validator (Application Layer)
```csharp
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MinimumLength(2).MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không đúng định dạng");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
            .Matches("[A-Z]").WithMessage("Phải có ít nhất 1 chữ hoa")
            .Matches("[0-9]").WithMessage("Phải có ít nhất 1 chữ số");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Xác nhận mật khẩu không khớp");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(0[3|5|7|8|9])+([0-9]{8})$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Số điện thoại không hợp lệ");
    }
}
```

### Template 4: Controller (WebApi Layer)
```csharp
[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;
    public AuthController(ISender sender) => _sender = sender;

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<RegisterResponse>.Success("Đăng ký thành công", result.Value))
            : BadRequest(ApiResponse<RegisterResponse>.Failure(result.Error.Message));
    }
}
```

---

## 6. ANTI-PATTERNS — TUYỆT ĐỐI KHÔNG làm

```
❌ KHÔNG gọi AppDbContext trực tiếp trong Application Layer
❌ KHÔNG inject HttpContext vào Handler
❌ KHÔNG dùng static class chứa business logic
❌ KHÔNG trả về IQueryable ra ngoài Repository
❌ KHÔNG dùng public set; trong Entity (dùng private set;)
❌ KHÔNG dùng Task.Result hoặc .Wait() — luôn dùng await
❌ KHÔNG throw Exception để điều hướng luồng — dùng Result<T>
❌ KHÔNG bỏ qua CancellationToken trong các hàm async
❌ KHÔNG để logic trong Controller
❌ KHÔNG tạo UserRole ngoài: Customer, ClaimOfficer (MVP scope)
❌ KHÔNG lưu plain-text password — luôn hash bằng BCrypt
```
