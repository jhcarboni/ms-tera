using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using MsTera.Shared.Infrastructure.Auth;

namespace MsTera.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapGet("/login-google", () =>
        {
            var properties = new AuthenticationProperties { RedirectUri = "/api/auth/google-callback" };
            return Results.Challenge(properties, [GoogleDefaults.AuthenticationScheme]);
        });

        group.MapGet("/google-callback", async (HttpContext context, IJwtTokenGenerator tokenGenerator) =>
        {
            var result = await context.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Results.Unauthorized();

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);
            var googleId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(email))
                return Results.BadRequest("Email claims missing from Google.");

            // Aqui você deve buscar no banco de dados se o usuário existe.
            // Se não existir, criar um novo usuário no módulo Users.
            // Por enquanto, geramos um token fictício usando o googleId como userId.
            var token = tokenGenerator.GenerateToken(email, googleId ?? Guid.NewGuid().ToString());

            return Results.Ok(new
            {
                Message = "Login com sucesso!",
                User = name,
                Email = email,
                Token = token
            });
        });
        group.MapGet("/me", (ClaimsPrincipal user) =>
        {
            return Results.Ok(new
            {
                Email = user.FindFirstValue(ClaimTypes.Email),
                Name = user.FindFirstValue(ClaimTypes.Name),
                Id = user.FindFirstValue(ClaimTypes.NameIdentifier)
            });
        }).RequireAuthorization();
    }
}
