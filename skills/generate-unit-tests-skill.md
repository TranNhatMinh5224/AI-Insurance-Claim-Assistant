---
skill_name: "Generate Unit Tests - Clean Architecture Backend"
version: "1.0"
tech_stack: "xUnit, Moq, FluentAssertions, FluentValidation.TestHelper, .NET 10"
project: "AI-Insurance-Claim-Assistant / Backend.UnitTests"
test_project_path: "src/backend/tests/Backend.UnitTests"
applies_to: "RegisterCommandHandler, Validators, Controllers (và mọi feature tương tự)"
last_updated: "2026-07-28"
---

# Agent Skill: Tự động sinh Unit Tests (Clean Architecture)

## 1. CONTEXT — Khi nào kích hoạt Skill này

### ✅ Kích hoạt khi người dùng yêu cầu:
- "Viết unit test cho feature X"
- "Tạo test cho Handler / Validator / Controller"
- "Sinh test scenarios cho chức năng Y"
- "Kiểm tra coverage cho feature Z"

### ❌ KHÔNG áp dụng khi:
- Yêu cầu viết Integration Test (có kết nối DB thật)
- Yêu cầu viết E2E / API Test
- Yêu cầu test cho Python/AI Service

---

## 2. CẤU TRÚC THƯ MỤC TEST (Folder Structure Convention)

```
Backend.UnitTests/
├── Features/
│   └── {FeatureDomain}/          ← Tên domain (VD: Auth, Claims, Vehicles)
│       └── {FeatureName}/        ← Tên feature (VD: Register, Login, CreateClaim)
│           ├── {Name}CommandHandlerTests.cs
│           └── {Name}CommandValidatorTests.cs
│
└── Controllers/
    └── {Name}ControllerTests.cs
```

**RULE:** Cấu trúc thư mục test PHẢI mirror theo cấu trúc của `Backend.Application/Features/`.  
Nếu source code nằm ở `Features/Auth/Register/`, test nằm ở `Features/Auth/Register/`.

---

## 3. NAMING CONVENTIONS (Quy ước đặt tên)

### Tên file test:
```
{ClassName}Tests.cs

VD:
RegisterCommandHandlerTests.cs
RegisterCommandValidatorTests.cs
AuthControllerTests.cs
```

### Tên method test — Công thức BẮT BUỘC:
```
{Method}_{Condition}_{ExpectedBehavior}

VD:
Handle_WhenEmailAlreadyExists_ShouldReturnFailure()
Handle_WithValidData_ShouldReturnSuccess()
Validate_WhenEmailEmpty_ShouldHaveError()
Register_WhenHandlerReturnsSuccess_ShouldReturn200Ok()
```

### Display name (bắt buộc có):
```csharp
[Fact(DisplayName = "TC-{số}: {mô tả bằng tiếng Việt rõ ràng}")]
```

---

## 4. RULES — Quy tắc Bắt buộc

### GROUP A: Cấu trúc Class Test

#### RULE A1: Mỗi class test PHẢI có phần comment kịch bản ở đầu
```csharp
/// <summary>
/// Unit Tests cho {ClassName}
///
/// Kịch bản kiểm thử (Test Scenarios):
/// ✅ TC-01: {Mô tả happy path 1}
/// ✅ TC-02: {Mô tả happy path 2}
/// ❌ TC-03: {Mô tả failure case 1}
/// ❌ TC-04: {Mô tả failure case 2}
/// </summary>
```
- `✅` = happy path (dữ liệu hợp lệ, kỳ vọng thành công)
- `❌` = failure case (dữ liệu lỗi, kỳ vọng thất bại)

#### RULE A2: Mỗi class test PHẢI có constructor khởi tạo mocks
```csharp
public class RegisterCommandHandlerTests
{
    // Khai báo tất cả mocks ở đây
    private readonly Mock<IUserRepository> _userRepositoryMock;
    // ...
    private readonly RegisterCommandHandler _handler; // ← object được test

    public RegisterCommandHandlerTests() // ← Khởi tạo trong constructor
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        // ...
        _handler = new RegisterCommandHandler(_userRepositoryMock.Object, ...);
    }
}
```

#### RULE A3: PHẢI có Helper method tạo dữ liệu hợp lệ mặc định
```csharp
// Helper luôn là static, tạo dữ liệu "hoàn toàn hợp lệ"
private static RegisterCommand ValidCommand(string email = "test@gmail.com") =>
    new("Nguyễn Văn A", email, "Abc@1234", "Abc@1234", "0912345678");
```

#### RULE A4: Nhóm test cases theo NHÓM logic, dùng comment phân cách
```csharp
// ══════════════════════════════════════════════════════
// NHÓM 1: Happy Path (Dữ liệu hợp lệ)
// ══════════════════════════════════════════════════════

// ══════════════════════════════════════════════════════
// NHÓM 2: Email Validation
// ══════════════════════════════════════════════════════
```

---

### GROUP B: Cấu trúc từng test method — AAA Pattern

#### RULE B1: LUÔN dùng AAA Pattern (Arrange → Act → Assert)
```csharp
[Fact(DisplayName = "TC-01: ...")]
public async Task Handle_WithValidData_ShouldReturnSuccess()
{
    // Arrange  ← Chuẩn bị dữ liệu và mock
    _userRepositoryMock
        .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    // Act  ← Gọi method cần test
    var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

    // Assert  ← Kiểm tra kết quả
    result.IsSuccess.Should().BeTrue("vì email hợp lệ và chưa tồn tại");
}
```

#### RULE B2: Assert PHẢI có message giải thích lý do (WithMessage trong FluentAssertions)
```csharp
// ✅ ĐÚNG — Có message giải thích
result.IsSuccess.Should().BeTrue("vì email chưa tồn tại và dữ liệu hợp lệ");

// ❌ SAI — Không có message
result.IsSuccess.Should().BeTrue();
```

#### RULE B3: Dùng `[Theory] + [InlineData]` cho nhiều input tương tự
```csharp
// ✅ ĐÚNG — Gộp nhiều invalid emails vào 1 test
[Theory(DisplayName = "TC-V07: Email sai định dạng → Có lỗi")]
[InlineData("notanemail")]
[InlineData("missing@")]
[InlineData("@nodomain.com")]
public void Validate_WhenEmailInvalidFormat_ShouldHaveError(string invalidEmail)
{ ... }

// ❌ SAI — Tạo riêng 3 test method cho cùng 1 kịch bản
```

---

### GROUP C: Rules riêng cho Handler Tests

#### RULE C1: Kịch bản BẮT BUỘC cho mọi Handler — Happy Path
Mọi Handler test PHẢI có đủ 5 kịch bản sau:
```
✅ TC-H1: [Điều kiện hợp lệ] → IsSuccess = true
✅ TC-H2: [Điều kiện hợp lệ] → Side effect 1 được gọi (VD: AddAsync Times.Once)
✅ TC-H3: [Điều kiện hợp lệ] → Side effect 2 được gọi (VD: SaveChanges Times.Once)
✅ TC-H4: [Điều kiện hợp lệ] → Password được hash (nếu có)
✅ TC-H5: [Điều kiện hợp lệ] → Response data chứa đúng thông tin
```

#### RULE C2: Kịch bản BẮT BUỘC cho mọi Handler — Failure Path
Mọi Handler test PHẢI có đủ 4 kịch bản sau:
```
❌ TC-F1: [Điều kiện lỗi] → IsFailure = true
❌ TC-F2: [Điều kiện lỗi] → Error.Code chính xác (VD: "User.EmailAlreadyExists")
❌ TC-F3: [Điều kiện lỗi] → Side effect KHÔNG được gọi (VD: AddAsync Times.Never)
❌ TC-F4: [Điều kiện lỗi] → SaveChanges KHÔNG được gọi (Times.Never)
```

#### RULE C3: Verify side effects với Times.Once / Times.Never
```csharp
// Verify ĐƯỢC GỌI
_userRepositoryMock.Verify(
    r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
    Times.Once, "Phải persist user vào DB");

// Verify KHÔNG ĐƯỢC GỌI
_unitOfWorkMock.Verify(
    u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
    Times.Never, "Không được commit khi có lỗi");
```

---

### GROUP D: Rules riêng cho Validator Tests

#### RULE D1: Dùng `FluentValidation.TestHelper` — TestValidate + ShouldHaveValidationErrorFor
```csharp
// ✅ ĐÚNG — Dùng TestHelper
var result = _validator.TestValidate(command);
result.ShouldHaveValidationErrorFor(x => x.Email);
result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);

// ❌ SAI — Validate thủ công
var result = _validator.Validate(command);
Assert.False(result.IsValid); // Không rõ lỗi ở field nào
```

#### RULE D2: Kịch bản BẮT BUỘC cho mọi Validator

Mỗi field trong Command đều phải có test:
```
✅ TC-V1: Tất cả hợp lệ → ShouldNotHaveAnyValidationErrors()
✅ TC-V2: Field optional bỏ trống → Không có lỗi

❌ TC-V(n): [Field] để trống → Có lỗi
❌ TC-V(n): [Field] sai định dạng → Có lỗi
❌ TC-V(n): [Field] quá ngắn/dài → Có lỗi
```

#### RULE D3: Dùng `with` expression để chỉ thay đổi 1 field từ ValidCommand
```csharp
// ✅ ĐÚNG — Chỉ thay đổi field cần test
var command = ValidCommand() with { Email = "invalid-email" };

// ❌ SAI — Tạo lại toàn bộ command
var command = new RegisterCommand("", "invalid", "", "", "");
// → Không biết lỗi do field nào
```

---

### GROUP E: Rules riêng cho Controller Tests

#### RULE E1: Kịch bản BẮT BUỘC cho mọi Controller Action

```
✅ TC-C1: Handler trả Success → HTTP Status Code đúng (200/201)
✅ TC-C2: Handler trả Success → Response.Success = true
✅ TC-C3: Handler trả Success → Response.Data chứa dữ liệu đúng
❌ TC-C4: Handler trả Failure → HTTP Status Code đúng (400/404/409)
❌ TC-C5: Handler trả Failure → Response.Success = false
❌ TC-C6: Handler trả Failure → Response.Message chứa error message
```

#### RULE E2: Mock ISender, không mock toàn bộ pipeline
```csharp
// ✅ ĐÚNG — Mock ISender
private readonly Mock<ISender> _senderMock;
_senderMock
    .Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(Result<RegisterResponse>.Success(validResponse));

// ❌ SAI — Test toàn bộ pipeline trong Controller test
```

#### RULE E3: Kiểm tra cả kiểu object của ActionResult
```csharp
// ✅ ĐÚNG — Verify đúng kiểu
actionResult.Should().BeOfType<OkObjectResult>("Handler thành công phải là 200 OK");
actionResult.Should().BeOfType<BadRequestObjectResult>("Handler thất bại phải là 400");

// Lấy value từ ActionResult
var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<RegisterResponse>>().Subject;
```

---

## 5. EXECUTION PROCESS — Thứ tự khi được yêu cầu viết test

```
Bước 1: Đọc source code cần test
  → Xem Handler/Validator/Controller đang test cái gì
  → Xác định tất cả nhánh logic (if/else, success/failure)
  → Liệt kê toàn bộ kịch bản có thể xảy ra

Bước 2: Thiết kế danh sách kịch bản (Test Plan)
  → Happy path: bao nhiêu trường hợp?
  → Failure path: bao nhiêu trường hợp?
  → Edge cases: các giá trị biên (empty, null, max length)

Bước 3: Viết comment kịch bản ở đầu class
  → Liệt kê TC-01, TC-02... với ✅/❌

Bước 4: Viết test methods theo thứ tự
  → Happy path trước, failure cases sau
  → Nhóm test theo NHÓM logic

Bước 5: Đảm bảo coverage tối thiểu
  → Mỗi Handler: tối thiểu 9 TCs (5 happy + 4 failure)
  → Mỗi Validator: tối thiểu 1 TC/rule + 1 happy path
  → Mỗi Controller action: tối thiểu 6 TCs (3 success + 3 failure)
```

---

## 6. CODE TEMPLATES

### Template 1: Handler Test Class
```csharp
using Backend.Application.Abstractions;
using Backend.Application.Features.Auth.{Feature};
using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Backend.UnitTests.Features.{Domain}.{Feature};

/// <summary>
/// Unit Tests cho {Feature}CommandHandler
///
/// Kịch bản kiểm thử (Test Scenarios):
/// ✅ TC-01: {Happy path description}
/// ✅ TC-02: Side effect 1 được gọi đúng
/// ✅ TC-03: Side effect 2 được gọi đúng
/// ✅ TC-04: Password được hash (nếu applicable)
/// ✅ TC-05: Response data chứa đúng thông tin
/// ❌ TC-06: {Failure condition} → IsFailure = true
/// ❌ TC-07: {Failure condition} → Error Code chính xác
/// ❌ TC-08: {Failure condition} → AddAsync không được gọi
/// ❌ TC-09: {Failure condition} → SaveChanges không được gọi
/// </summary>
public class {Feature}CommandHandlerTests
{
    private readonly Mock<I{Entity}Repository> _{entityLower}RepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly {Feature}CommandHandler _handler;

    public {Feature}CommandHandlerTests()
    {
        _{entityLower}RepositoryMock = new Mock<I{Entity}Repository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new {Feature}CommandHandler(_{entityLower}RepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static {Feature}Command ValidCommand() =>
        new( /* fill valid args */ );

    // ══════════════════════════════════════════════════════
    // NHÓM 1: Happy Path
    // ══════════════════════════════════════════════════════

    [Fact(DisplayName = "TC-01: Dữ liệu hợp lệ → IsSuccess = true")]
    public async Task Handle_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        // Act
        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeTrue("vì ...");
    }

    // ══════════════════════════════════════════════════════
    // NHÓM 2: Failure Cases
    // ══════════════════════════════════════════════════════

    [Fact(DisplayName = "TC-06: {Failure condition} → IsFailure = true")]
    public async Task Handle_When{Condition}_ShouldReturnFailure()
    {
        // Arrange
        // Act
        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);
        // Assert
        result.IsFailure.Should().BeTrue("vì ...");
    }
}
```

### Template 2: Validator Test Class
```csharp
using Backend.Application.Features.Auth.{Feature};
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Backend.UnitTests.Features.{Domain}.{Feature};

/// <summary>
/// Unit Tests cho {Feature}CommandValidator
///
/// Kịch bản kiểm thử:
/// ✅ TC-V01: Dữ liệu hoàn toàn hợp lệ → Không có lỗi
/// ❌ TC-V02: {Field} để trống → Có lỗi
/// ❌ TC-V03: {Field} sai định dạng → Có lỗi
/// </summary>
public class {Feature}CommandValidatorTests
{
    private readonly {Feature}CommandValidator _validator = new();

    private static {Feature}Command ValidCommand() =>
        new( /* fill all valid values */ );

    // ══════════════════════════════════════════════════════
    // NHÓM 1: Happy Path
    // ══════════════════════════════════════════════════════

    [Fact(DisplayName = "TC-V01: Dữ liệu hoàn toàn hợp lệ → Không có lỗi nào")]
    public void Validate_WithAllValidData_ShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ══════════════════════════════════════════════════════
    // NHÓM 2: {Field Name} Validation
    // ══════════════════════════════════════════════════════

    [Theory(DisplayName = "TC-V02: {Field} sai → Có lỗi")]
    [InlineData("value1")]
    [InlineData("value2")]
    public void Validate_When{Field}{Condition}_ShouldHaveError(string input)
    {
        var command = ValidCommand() with { {Field} = input };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.{Field});
    }
}
```

### Template 3: Controller Test Class
```csharp
using Backend.Application.Features.Auth.{Feature};
using Backend.Domain.Common;
using Backend.WebApi.Common;
using Backend.WebApi.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Backend.UnitTests.Controllers;

/// <summary>
/// Unit Tests cho {Name}Controller
///
/// Kịch bản kiểm thử:
/// ✅ TC-C01: Handler Success → 200 OK
/// ✅ TC-C02: Handler Success → success = true
/// ✅ TC-C03: Handler Success → data chứa dữ liệu đúng
/// ❌ TC-C04: Handler Failure → 400 BadRequest
/// ❌ TC-C05: Handler Failure → success = false
/// ❌ TC-C06: Handler Failure → message chứa lỗi
/// </summary>
public class {Name}ControllerTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly {Name}Controller _controller;

    public {Name}ControllerTests()
    {
        _senderMock = new Mock<ISender>();
        _controller = new {Name}Controller(_senderMock.Object);
    }

    [Fact(DisplayName = "TC-C01: Handler Success → 200 OK")]
    public async Task {Action}_WhenHandlerReturnsSuccess_ShouldReturn200Ok()
    {
        _senderMock
            .Setup(s => s.Send(It.IsAny<{Feature}Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<{Feature}Response>.Success(/* valid response */));

        var result = await _controller.{Action}(/* command */, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>("Handler thành công phải trả về HTTP 200");
    }
}
```

---

## 7. COVERAGE REQUIREMENTS — Yêu cầu Coverage tối thiểu

| Loại Test | Số TC tối thiểu | Bắt buộc cover |
|---|---|---|
| **Handler Tests** | 9 TC | 5 happy + 4 failure |
| **Validator Tests** | 1 TC/field + 1 happy path | Mọi ValidationRule |
| **Controller Tests** | 6 TC/action | 3 success + 3 failure |

---

## 8. ANTI-PATTERNS — TUYỆT ĐỐI KHÔNG làm trong Unit Test

```
❌ KHÔNG dùng new() trực tiếp cho dependencies (phải dùng Mock)
❌ KHÔNG để test method quá 30 dòng — tách ra nếu quá dài
❌ KHÔNG Assert nhiều hơn 3 điều trong 1 test method
❌ KHÔNG bỏ qua DisplayName trong [Fact] và [Theory]
❌ KHÔNG viết Assert.True(result.IsSuccess) — dùng FluentAssertions
❌ KHÔNG tạo shared state giữa các test (mỗi test độc lập hoàn toàn)
❌ KHÔNG gọi DB thật / file system / external API trong unit test
❌ KHÔNG bỏ qua kịch bản "side effect KHÔNG được gọi" (Times.Never)
❌ KHÔNG viết test chỉ để tăng coverage mà không có assertion có ý nghĩa
❌ KHÔNG đặt tên test mơ hồ như Test1(), TestRegister(), TestSuccess()
```
