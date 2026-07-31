using Backend.Application.Features.Documents.CreateDocument;
using Backend.Application.Features.Documents.GetMyDocuments;
using Backend.Application.Features.Documents.UploadDraft;
using Backend.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/documents")]
[Authorize(Roles = Roles.Customer)] // Chỉ Customer mới được upload giấy tờ của mình
public sealed class DocumentsController : ControllerBase
{
    private readonly ISender _sender;

    public DocumentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// [Bước 1] Tải ảnh lên kho nháp (Draft). Trả về URL preview để xem trước.
    /// Database không bị ảnh hưởng ở bước này.
    /// </summary>
    [HttpPost("upload-draft")]
    [RequestSizeLimit(10 * 1024 * 1024)] // Giới hạn 10MB
    [ProducesResponseType(typeof(ApiResponse<UploadDocumentDraftResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UploadDocumentDraftResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadDraft(IFormFile file, CancellationToken cancellationToken)
    {
        var command = new UploadDocumentDraftCommand(file);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(ApiResponse<UploadDocumentDraftResponse>.SuccessResult("Tải ảnh lên thành công! Hãy xem trước và xác nhận.", result.Value))
            : BadRequest(ApiResponse<UploadDocumentDraftResponse>.FailureResult(result.Error.Message));
    }

    /// <summary>
    /// [Bước 2] Xác nhận lưu giấy tờ: chuyển file từ Draft sang Real và ghi vào Database.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateCustomerDocumentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CreateCustomerDocumentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmDocument(
        [FromBody] CreateCustomerDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("Auth")
                ? Unauthorized(ApiResponse<CreateCustomerDocumentResponse>.FailureResult(result.Error.Message))
                : BadRequest(ApiResponse<CreateCustomerDocumentResponse>.FailureResult(result.Error.Message));
        }

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<CreateCustomerDocumentResponse>.SuccessResult("Lưu giấy tờ thành công!", result.Value));
    }

    /// <summary>
    /// [Customer] Lấy danh sách các giấy tờ mà khách hàng đã tải lên (CCCD, Đăng ký xe).
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<List<GetMyDocumentsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyDocuments(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyDocumentsQuery(), cancellationToken);

        return Ok(ApiResponse<List<GetMyDocumentsResponse>>.SuccessResult(
            "Lấy danh sách giấy tờ thành công.", result.Value));
    }
}
