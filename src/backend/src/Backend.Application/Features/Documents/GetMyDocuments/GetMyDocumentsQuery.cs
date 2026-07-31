using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Documents.GetMyDocuments;

public sealed record GetMyDocumentsQuery() : IRequest<Result<List<GetMyDocumentsResponse>>>;
