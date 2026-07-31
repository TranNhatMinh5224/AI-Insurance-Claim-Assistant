using Backend.Application.Features.Staff.Claims.GetAllClaims;
using Backend.Application.Features.Staff.Claims.GetClaimById;
using Backend.Application.Features.Staff.Claims.GetClaimById;
using Backend.Application.Features.Staff.Claims.ProcessClaim;
using Backend.Application.Features.Staff.Policies.ApprovePolicy;
using Backend.Application.Features.Staff.Policies.GetPendingPolicies;
using Backend.Application.Features.Staff.Policies.RejectPolicy;
using Backend.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/staff")]
[Authorize(Roles = Roles.StaffAndAdmin)] // Nhân viên và Admin đều dùng được
public sealed class StaffController : ControllerBase
{
    private readonly ISender _sender;

    public StaffController(ISender sender)
    {
        _sender = sender;
    }

    // ─────────────── Policies Management ───────────────────────────

    /// <summary>
    /// [Staff] Lấy danh sách các hợp đồng mới đang chờ phê duyệt.
    /// </summary>
    [HttpGet("policies/pending")]
    [ProducesResponseType(typeof(ApiResponse<List<GetPendingPoliciesResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingPolicies(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPendingPoliciesQuery(), cancellationToken);

        return Ok(ApiResponse<List<GetPendingPoliciesResponse>>.SuccessResult(
            "Lấy danh sách hợp đồng chờ duyệt thành công", result.Value));
    }

    /// <summary>
    /// [Staff/Admin] Duyệt hợp đồng bảo hiểm. Chuyển trạng thái sang Active và cấp E-Policy.
    /// </summary>
    [HttpPost("policies/{id:guid}/approve")]
    [ProducesResponseType(typeof(ApiResponse<ApprovePolicyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ApprovePolicyResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ApprovePolicyResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApprovePolicy([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ApprovePolicyCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("Policy.NotFound")
                ? NotFound(ApiResponse<ApprovePolicyResponse>.FailureResult(result.Error.Message))
                : Conflict(ApiResponse<ApprovePolicyResponse>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<ApprovePolicyResponse>.SuccessResult("Duyệt hợp đồng thành công!", result.Value));
    }

    /// <summary>
    /// [Staff/Admin] Từ chối hợp đồng bảo hiểm (Ví dụ: phát hiện gian lận giấy tờ).
    /// </summary>
    [HttpPost("policies/{id:guid}/reject")]
    [ProducesResponseType(typeof(ApiResponse<RejectPolicyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RejectPolicyResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<RejectPolicyResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectPolicy([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RejectPolicyCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("Policy.NotFound")
                ? NotFound(ApiResponse<RejectPolicyResponse>.FailureResult(result.Error.Message))
                : Conflict(ApiResponse<RejectPolicyResponse>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<RejectPolicyResponse>.SuccessResult("Từ chối hợp đồng thành công!", result.Value));
    }

    // ─────────────── Claims Management ───────────────────────────

    /// <summary>
    /// [Staff/Admin] Lấy danh sách toàn bộ hồ sơ yêu cầu bồi thường.
    /// (Sau này sẽ bổ sung filter trạng thái / nhãn cảnh báo AI).
    /// </summary>
    [HttpGet("claims")]
    [ProducesResponseType(typeof(ApiResponse<List<GetAllClaimsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllClaims(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllClaimsQuery(), cancellationToken);

        return Ok(ApiResponse<List<GetAllClaimsResponse>>.SuccessResult(
            "Lấy danh sách hồ sơ bồi thường thành công", result.Value));
    }

    /// <summary>
    /// [Staff/Admin] Xử lý hồ sơ bồi thường (Chốt Duyệt bồi thường hoặc Từ chối).
    /// </summary>
    [HttpPost("claims/{id:guid}/process")]
    [ProducesResponseType(typeof(ApiResponse<ProcessClaimResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProcessClaimResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessClaim([FromRoute] Guid id, [FromBody] ProcessClaimCommand command, CancellationToken cancellationToken)
    {
        // Gắn ID từ URL vào Command
        var finalCommand = command with { ClaimId = id };
        var result = await _sender.Send(finalCommand, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<ProcessClaimResponse>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<ProcessClaimResponse>.SuccessResult(
            $"Đã cập nhật trạng thái hồ sơ thành {result.Value.Status}", result.Value));
    }

    /// <summary>
    /// [Staff/Admin] Lấy chi tiết hồ sơ bồi thường (Bao gồm danh sách ảnh hiện trường và báo cáo AI).
    /// </summary>
    [HttpGet("claims/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GetClaimByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<GetClaimByIdResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClaimById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetClaimByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<GetClaimByIdResponse>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<GetClaimByIdResponse>.SuccessResult("Lấy chi tiết hồ sơ thành công.", result.Value));
    }
}
