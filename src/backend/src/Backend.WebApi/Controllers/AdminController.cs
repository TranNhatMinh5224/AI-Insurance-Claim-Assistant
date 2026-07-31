using Backend.Application.Features.Admin.Packages.CreatePackage;
using Backend.Application.Features.Admin.Packages.DeactivatePackage;
using Backend.Application.Features.Admin.Packages.GetAllPackages;
using Backend.Application.Features.Admin.PolicyTerms.ConfirmPolicyTerm;
using Backend.Application.Features.Admin.PolicyTerms.UploadPolicyTermDraft;
using Backend.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = Roles.SuperAdmin)] // Toàn bộ controller chỉ cho SuperAdmin
public sealed class AdminController : ControllerBase
{
    private readonly ISender _sender;

    public AdminController(ISender sender)
    {
        _sender = sender;
    }

    // ─────────────── Insurance Packages ───────────────────────────

    /// <summary>
    /// [SuperAdmin] Lấy danh sách TOÀN BỘ gói bảo hiểm (kể cả đã bị khóa).
    /// </summary>
    [HttpGet("packages")]
    [ProducesResponseType(typeof(ApiResponse<List<GetAllPackagesResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPackages(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllPackagesQuery(), cancellationToken);
        return Ok(ApiResponse<List<GetAllPackagesResponse>>.SuccessResult(
            "Lấy danh sách gói bảo hiểm thành công", result.Value));
    }

    /// <summary>
    /// [SuperAdmin] Tạo gói bảo hiểm mới.
    /// </summary>
    [HttpPost("packages")]
    [ProducesResponseType(typeof(ApiResponse<CreatePackageResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CreatePackageResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CreatePackageResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePackage(
        [FromBody] CreatePackageCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("Package")
                ? Conflict(ApiResponse<CreatePackageResponse>.FailureResult(result.Error.Message))
                : BadRequest(ApiResponse<CreatePackageResponse>.FailureResult(result.Error.Message));
        }

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<CreatePackageResponse>.SuccessResult("Tạo gói bảo hiểm thành công!", result.Value));
    }

    /// <summary>
    /// [SuperAdmin] Khóa gói bảo hiểm — gói sẽ biến mất khỏi danh sách của Customer.
    /// Các hợp đồng cũ đang dùng gói này KHÔNG bị ảnh hưởng (Price Versioning).
    /// </summary>
    [HttpPatch("packages/{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse<DeactivatePackageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DeactivatePackageResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<DeactivatePackageResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeactivatePackage(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeactivatePackageCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                var c when c.StartsWith("Package.NotFound") =>
                    NotFound(ApiResponse<DeactivatePackageResponse>.FailureResult(result.Error.Message)),
                _ => Conflict(ApiResponse<DeactivatePackageResponse>.FailureResult(result.Error.Message))
            };
        }

        return Ok(ApiResponse<DeactivatePackageResponse>.SuccessResult(
            "Gói bảo hiểm đã được khóa thành công.", result.Value));
    }

    // ─────────────── Policy Terms ──────────────────────────────────

    /// <summary>
    /// [SuperAdmin] Bước 1: Upload PDF điều khoản bảo hiểm lên kho nháp.
    /// Trả về URL preview để Admin đọc trước khi xác nhận.
    /// </summary>
    [HttpPost("policy-terms/upload-draft")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    [ProducesResponseType(typeof(ApiResponse<UploadPolicyTermDraftResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UploadPolicyTermDraftResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadPolicyTermDraft(
        [FromForm] Guid packageId,
        [FromForm] string version,
        IFormFile pdfFile,
        CancellationToken cancellationToken)
    {
        var command = new UploadPolicyTermDraftCommand(packageId, version, pdfFile);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(ApiResponse<UploadPolicyTermDraftResponse>.SuccessResult("Upload PDF thành công! Hãy xem trước rồi xác nhận.", result.Value))
            : BadRequest(ApiResponse<UploadPolicyTermDraftResponse>.FailureResult(result.Error.Message));
    }

    /// <summary>
    /// [SuperAdmin] Bước 2: Xác nhận điều khoản — commit PDF từ draft → real và lưu vào Database.
    /// </summary>
    [HttpPost("policy-terms")]
    [ProducesResponseType(typeof(ApiResponse<ConfirmPolicyTermResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ConfirmPolicyTermResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmPolicyTerm(
        [FromBody] ConfirmPolicyTermCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("NotFound")
                ? NotFound(ApiResponse<ConfirmPolicyTermResponse>.FailureResult(result.Error.Message))
                : BadRequest(ApiResponse<ConfirmPolicyTermResponse>.FailureResult(result.Error.Message));
        }

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<ConfirmPolicyTermResponse>.SuccessResult("Lưu điều khoản bảo hiểm thành công!", result.Value));
    }
}
