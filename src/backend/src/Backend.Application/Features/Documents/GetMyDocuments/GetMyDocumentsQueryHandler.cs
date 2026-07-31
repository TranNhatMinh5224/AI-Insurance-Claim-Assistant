using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Documents.GetMyDocuments;

internal sealed class GetMyDocumentsQueryHandler 
    : IRequestHandler<GetMyDocumentsQuery, Result<List<GetMyDocumentsResponse>>>
{
    private readonly ICustomerDocumentRepository _documentRepo;
    private readonly ICurrentUserService _currentUser;

    public GetMyDocumentsQueryHandler(ICustomerDocumentRepository documentRepo, ICurrentUserService currentUser)
    {
        _documentRepo = documentRepo;
        _currentUser = currentUser;
    }

    public async Task<Result<List<GetMyDocumentsResponse>>> Handle(GetMyDocumentsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.GetUserIdOrThrow();
        var documents = await _documentRepo.GetByUserIdAsync(userId, ct);

        var response = documents.Select(d => new GetMyDocumentsResponse(
            d.Id,
            d.DocumentType.ToString(),
            d.FileUrl,
            d.Status.ToString(),
            d.OcrData,
            d.CreatedAt
        )).ToList();

        return Result<List<GetMyDocumentsResponse>>.Success(response);
    }
}
