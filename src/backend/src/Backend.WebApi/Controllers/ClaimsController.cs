using Backend.Application.Features.Claims.GetMyClaims;
using Backend.Application.Features.Claims.SubmitClaim;
using Backend.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/claims")]
[Authorize] // Có thể cả Customer (nộp/xem của mình) và Staff (xem)
public sealed class ClaimsController : ControllerBase
{
    private readonly ISender _sender;

    public ClaimsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// [Customer] Khách hàng nộp hồ sơ yêu cầu bồi thường (upload kèm ảnh).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Customer)]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50MB tổng cộng
    [ProducesResponseType(typeof(ApiResponse<SubmitClaimResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SubmitClaimResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SubmitClaimResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<SubmitClaimResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitClaim(
        [FromForm] Guid policyId,
        [FromForm] string incidentDescription,
        [FromForm] List<IFormFile> evidenceFiles,
        CancellationToken cancellationToken)
    {
        var command = new SubmitClaimCommand(policyId, incidentDescription, evidenceFiles);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                var c when c.StartsWith("Policy.NotFound") =>
                    NotFound(ApiResponse<SubmitClaimResponse>.FailureResult(result.Error.Message)),
                var c when c.StartsWith("Policy.NotActive") =>
                    Conflict(ApiResponse<SubmitClaimResponse>.FailureResult(result.Error.Message)),
                _ => BadRequest(ApiResponse<SubmitClaimResponse>.FailureResult(result.Error.Message))
            };
        }

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<SubmitClaimResponse>.SuccessResult("Nộp hồ sơ bồi thường thành công! AI đang tiến hành đánh giá.", result.Value));
    }

    /// <summary>
    /// [Customer] Xem lịch sử và trạng thái các hồ sơ yêu cầu bồi thường của mình.
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(typeof(ApiResponse<List<GetMyClaimsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyClaims(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyClaimsQuery(), cancellationToken);

        return Ok(ApiResponse<List<GetMyClaimsResponse>>.SuccessResult(
            "Lấy lịch sử bồi thường thành công.", result.Value));
    }
}
