using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Documents.UploadDraft;

internal sealed class UploadDocumentDraftCommandHandler
    : IRequestHandler<UploadDocumentDraftCommand, Result<UploadDocumentDraftResponse>>
{
    private const string BucketName = "customer-documents";
    private readonly IFileStorageService _fileStorage;

    public UploadDocumentDraftCommandHandler(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<Result<UploadDocumentDraftResponse>> Handle(
        UploadDocumentDraftCommand request, CancellationToken ct)
    {
        var file = request.File;

        // Sinh tên file duy nhất để tránh trùng lặp trong MinIO
        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        await using var stream = file.OpenReadStream();

        // isDraft = true => lưu vào folder draft/
        var previewUrl = await _fileStorage.UploadFileAsync(
            stream, uniqueFileName, file.ContentType, BucketName, isDraft: true, ct);

        return Result<UploadDocumentDraftResponse>.Success(
            new UploadDocumentDraftResponse(uniqueFileName, previewUrl));
    }
}
