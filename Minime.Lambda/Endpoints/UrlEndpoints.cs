using Minime.Lambda.Models;
using Minime.Lambda.Services;

namespace Minime.Lambda.Endpoints;

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this WebApplication app)
    {
        app.MapPost("/link", async (
                NewLinkRequest request,
                HttpRequest httpRequest,
                IRecaptchaService recaptcha,
                IUrlRepository urls,
                IConfiguration configuration,
                CancellationToken cancellationToken) =>
            {
                var token = httpRequest.Headers.Authorization.FirstOrDefault()?
                    .Replace("Bearer ", "")
                    .Trim();

                if (!await recaptcha.ValidateAsync(token, cancellationToken))
                    return Results.StatusCode(StatusCodes.Status429TooManyRequests);

                var link = await urls.CreateAsync(request.Url, cancellationToken);

                // BaseUrl é a origem completa. O cliente usa o valor como veio,
                // sem montar o host por conta própria.
                var baseUrl = (configuration["BaseUrl"] ?? "https://link.minime.cloud").TrimEnd('/');

                return Results.Created(default(string), new { shortUrl = $"{baseUrl}/{link.UId}" });
            })
            .WithName("NewMinifiedUrl")
            .WithSummary("Gera uma nova URL encurtada (protegida por reCAPTCHA).")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status429TooManyRequests);

        app.MapGet("/{uid}", async (
                string uid,
                IUrlRepository urls,
                CancellationToken cancellationToken) =>
            {
                var link = await urls.GetAsync(uid, cancellationToken);

                return link is null ? Results.NotFound() : Results.Redirect(link.Url);
            })
            .WithName("RedirectToUrl")
            .WithSummary("Redireciona para a URL original.")
            .Produces(StatusCodes.Status302Found)
            .Produces(StatusCodes.Status404NotFound);
    }
}
