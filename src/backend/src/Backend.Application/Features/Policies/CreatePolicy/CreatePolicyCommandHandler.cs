using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using MediatR;

namespace Backend.Application.Features.Policies.CreatePolicy;

internal sealed class CreatePolicyCommandHandler 
    : IRequestHandler<CreatePolicyCommand, Result<CreatePolicyResponse>>
{
    private readonly ICarRepository _carRepo;
    private readonly IInsurancePackageRepository _packageRepo;
    private readonly IPolicyTermRepository _policyTermRepo;
    private readonly IInsurancePolicyRepository _policyRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreatePolicyCommandHandler(
        ICarRepository carRepo,
        IInsurancePackageRepository packageRepo,
        IPolicyTermRepository policyTermRepo,
        IInsurancePolicyRepository policyRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _carRepo = carRepo;
        _packageRepo = packageRepo;
        _policyTermRepo = policyTermRepo;
        _policyRepo = policyRepo;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<CreatePolicyResponse>> Handle(CreatePolicyCommand request, CancellationToken ct)
    {
        var userId = _currentUser.GetUserIdOrThrow();

        // 1. Kiểm tra Xe hợp lệ và thuộc sở hữu của User
        var car = await _carRepo.GetByIdAsync(request.CarId, ct);
        if (car is null || car.UserId != userId)
            return Result<CreatePolicyResponse>.Failure(
                Error.NotFound("Car.NotFound", "Không tìm thấy xe hợp lệ."));

        // 2. Xe chỉ được phép có 1 hợp đồng đang hiệu lực
        if (await _policyRepo.HasActivePolicyAsync(request.CarId, ct))
            return Result<CreatePolicyResponse>.Failure(
                Error.Conflict("Policy.AlreadyExists", "Xe này đã có hợp đồng bảo hiểm đang hiệu lực hoặc đang chờ duyệt."));

        // 3. Kiểm tra Gói bảo hiểm hợp lệ và còn mở bán
        var package = await _packageRepo.GetByIdAsync(request.PackageId, ct);
        if (package is null || !package.IsActive)
            return Result<CreatePolicyResponse>.Failure(
                Error.Conflict("Package.Invalid", "Gói bảo hiểm không tồn tại hoặc đã bị khóa."));

        // 4. Lấy Điều khoản (PDF) hiện hành của gói này
        var policyTerm = await _policyTermRepo.GetCurrentByPackageIdAsync(request.PackageId, ct);
        if (policyTerm is null)
            return Result<CreatePolicyResponse>.Failure(
                Error.Conflict("PolicyTerm.NotFound", "Gói bảo hiểm này chưa được cấu hình điều khoản pháp lý."));

        // 5. Tạo Hợp đồng mới (Chốt cứng Giá tiền và Bản PDF luật tại thời điểm mua)
        var policy = InsurancePolicy.Create(
            userId: userId,
            carId: request.CarId,
            packageId: request.PackageId,
            policyTermId: policyTerm.Id,
            premiumAmount: package.BasePrice
        );

        await _policyRepo.AddAsync(policy, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<CreatePolicyResponse>.Success(new CreatePolicyResponse(
            policy.Id,
            policy.CarId,
            policy.PackageId,
            policy.PremiumAmount,
            policy.Status.ToString()
        ));
    }
}
