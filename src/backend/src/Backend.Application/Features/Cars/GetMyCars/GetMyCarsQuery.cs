using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Cars.GetMyCars;

public sealed record GetMyCarsQuery() : IRequest<Result<List<GetMyCarsResponse>>>;
