using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using MediatR;

namespace Backend.Application.Features.Documents.CreateDocument;

internal sealed class CreateCustomerDocumentCommandHandler
    : IRequestHandler<CreateCustomerDocumentCommand, Result<CreateCustomerDocumentResponse>>
{
    private const string BucketName = "customer-documents";
    private readonly IFileStorageService _fileStorage;
    private readonly ICustomerDocumentRepository _documentRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateCustomerDocumentCommandHandler(
        IFileStorageService fileStorage,
        ICustomerDocumentRepository documentRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _fileStorage = fileStorage;
        _documentRepo = documentRepo;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<CreateCustomerDocumentResponse>> Handle(
        CreateCustomerDocumentCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<CreateCustomerDocumentResponse>.Failure(
                Error.Unauthorized("Auth.Unauthenticated", "Không xác định được người dùng."));

        // Commit file từ draft/ sang real/
        var realUrl = await _fileStorage.CommitFileAsync(request.DraftFileName, BucketName, ct);

        var document = CustomerDocument.Create(userId.Value, request.DocumentType, realUrl, imageHash: string.Empty);

        await _documentRepo.AddAsync(document, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<CreateCustomerDocumentResponse>.Success(new CreateCustomerDocumentResponse(
            document.Id,
            document.ImageUrl,
            document.DocumentType.ToString(),
            document.VerificationStatus.ToString()
        ));
    }
}
