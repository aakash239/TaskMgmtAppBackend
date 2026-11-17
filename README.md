# TaskMgmtAppBackend
Backend for POC Task Management Application.

## Local development

Requires .NET 10 and a SQL Server database deployed from
[`TaskManagement.Database`](TaskManagement.Database/README.md).
Supply `ConnectionStrings:defaultConnection` and the `Jwt` settings through
user secrets or environment variables. Do not commit credentials or signing keys.
JWT settings require an issuer, audience, a signing key of at least 32 UTF-8 bytes,
and `AccessTokenExpiryMinutes` equal to `15`.

```powershell
dotnet build TaskManagement.slnx
dotnet run --project TaskManagement.Api --launch-profile http
```

The development API runs at `http://localhost:5005` and permits credentialed
requests from `http://localhost:4200`. Use `localhost` for both applications.
The HTTP launch profile does not redirect to HTTPS in Development.
`Cors:AllowedOrigins` can override the exact frontend origins; outside Development,
configure HTTPS origins explicitly (for example, `Cors__AllowedOrigins__0`).

## Authentication contract

| Method | Route | Behavior |
|---|---|---|
| POST | `/api/auth/signup` | Create user and session; return public user/access-token data, set refresh cookie; `201` |
| POST | `/api/auth/login` | Validate credentials, create session and refresh cookie; `200` or `401` |
| POST | `/api/auth/refresh` | Read and rotate refresh cookie; return new access-token data; `200` or `401`; no request body or valid access JWT required |
| POST | `/api/auth/logout` | Revoke current session and clear cookie; idempotent `204` |
| POST | `/api/auth/logout-all` | Revoke this user's active sessions and clear cookie; requires bearer JWT; `204` |
| GET | `/api/auth/me` | Return validated access-token claims; requires bearer JWT |

Every authentication POST requires `X-CSRF: 1`. Browser requests must have an
allowed Origin. This custom header, exact origin checks, credentialed CORS and
SameSite cookies protect cookie-based mutations from cross-site requests.
Nonbrowser clients may omit Origin but must still send the custom header.

For Angular's existing HttpClient service, send these options on signup, login,
refresh and logout (no guard or interceptor is required):

```typescript
const authOptions = { withCredentials: true, headers: { 'X-CSRF': '1' } };
this.http.post<AuthResponse>('http://localhost:5005/api/auth/refresh', {}, authOptions);
```

Keep the returned access token in memory and send it as `Authorization: Bearer ...`
to protected endpoints. The public response keeps `userId`, `name`, `email`,
`accessToken`, and `expiresAtUtc`. Raw refresh tokens never appear in JSON.

The `refreshToken` cookie is HttpOnly, SameSite=Strict, host-only, and scoped to
`/api/auth`. It is Secure except for HTTP requests on `localhost` in Development,
the explicitly approved local-development exception. Issue and delete use the
same attributes. Other environments require HTTPS.

Sessions expire seven days after login/signup. Refresh rotation retains that
original deadline and stores only SHA-256 token hashes. Up to three unexpired,
unrevoked sessions are allowed; a new login revokes the oldest when necessary.
Reusing a rotated token returns `401` without revoking the session or clearing
the browser cookie. Logout and eviction block refresh; already issued JWTs remain
valid until their 15-minute expiry. Concurrent login/refresh hardening is deferred.

`AuthTests.http` and `TaskManagement.Api/TaskManagement.Api.http` provide manual
requests to run in the REST Client extension. There is no separate automated test
project. Use your local development configuration and SQL Server to check the
complete login, refresh and logout flow.
