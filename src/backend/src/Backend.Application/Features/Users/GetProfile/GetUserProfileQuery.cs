using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Users.GetProfile;

// RULE B1: Query = chỉ đọc, không thay đổi data
// RULE H1: Handler tự lấy UserId qua ICurrentUserService, không cần nhận từ Controller
public sealed record GetUserProfileQuery() : IRequest<Result<GetUserProfileResponse>>;
