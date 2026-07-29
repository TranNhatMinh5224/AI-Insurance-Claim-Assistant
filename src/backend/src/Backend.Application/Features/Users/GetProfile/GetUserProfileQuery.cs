using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Users.GetProfile;

// RULE B1: Query = chỉ đọc, không thay đổi data
// UserId được Controller đọc từ JWT claims và truyền vào
public sealed record GetUserProfileQuery(Guid UserId) : IRequest<Result<GetUserProfileResponse>>;
