using System.Text.Json.Serialization;

namespace TaskManagement.Application.DTOs;

// Internal result: controllers return Response and write the refresh secret only to a cookie.
public sealed record AuthSessionResultDto(
    AuthResponseDto Response,
    [property: JsonIgnore] string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc
);
