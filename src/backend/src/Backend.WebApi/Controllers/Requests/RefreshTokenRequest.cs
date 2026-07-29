namespace Backend.WebApi.Controllers.Requests;

public sealed record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken
);
