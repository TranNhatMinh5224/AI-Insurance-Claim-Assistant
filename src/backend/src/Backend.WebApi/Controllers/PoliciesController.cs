using Backend.Application.Features.Policies.CancelPolicy;
using Backend.Application.Features.Policies.CreatePolicy;
using Backend.Application.Features.Policies.GetMyPolicies;
using Backend.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/policies")]
[Authorize(Roles = Roles.Customer)] // Chỉ Khách hàng được mua bảo hiểm
public sealed class PoliciesController : ControllerBase
{
    private readonly ISender _sender;

    public PoliciesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// [Customer] Đăng ký mua gói bảo hiểm cho xe của mình.
    /// Hợp đồng sẽ được tạo ở trạng thái PendingApproval chờ Nhân viên duyệt.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreatePolicyResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CreatePolicyResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CreatePolicyResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<CreatePolicyResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePolicy(
        [FromBody] CreatePolicyCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                var c when c.StartsWith("Car.NotFound") =>
                    NotFound(ApiResponse<CreatePolicyResponse>.FailureResult(result.Error.Message)),
                var c when c.StartsWith("Policy") || c.StartsWith("Package") || c.StartsWith("PolicyTerm") =>
                    Conflict(ApiResponse<CreatePolicyResponse>.FailureResult(result.Error.Message)),
                _ => BadRequest(ApiResponse<CreatePolicyResponse>.FailureResult(result.Error.Message))
            };
        }

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<CreatePolicyResponse>.SuccessResult("Đăng ký mua bảo hiểm thành công! Vui lòng chờ phê duyệt.", result.Value));
    }

    /// <summary>
    /// [Customer] Lấy danh sách lịch sử Hợp đồng bảo hiểm của khách hàng (Pending, Active, Canceled, Expired).
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<List<GetMyPoliciesResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPolicies(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyPoliciesQuery(), cancellationToken);

        return Ok(ApiResponse<List<GetMyPoliciesResponse>>.SuccessResult(
            "Lấy danh sách hợp đồng thành công.", result.Value));
    }

    /// <summary>
    /// [Customer] Khách hàng yêu cầu hủy hợp đồng bảo hiểm đang có hiệu lực.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<CancelPolicyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CancelPolicyResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<CancelPolicyResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelPolicy([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CancelPolicyCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("Policy.NotFound")
                ? NotFound(ApiResponse<CancelPolicyResponse>.FailureResult(result.Error.Message))
                : Conflict(ApiResponse<CancelPolicyResponse>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<CancelPolicyResponse>.SuccessResult("Hủy hợp đồng thành công!", result.Value));
    }
}
