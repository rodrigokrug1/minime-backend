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
                var baseUrl = configuration["BaseUrl"] ?? "minime.cloud";

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

                return link is null ? Results.NoContent() : Results.Redirect(link.Url);
            })
            .WithName("RedirectToUrl")
            .WithSummary("Redireciona para a URL original.")
            .Produces(StatusCodes.Status302Found)
            .Produces(StatusCodes.Status204NoContent);

        // Endpoint herdado do antigo projeto Minime.Lambda.Get: retorna a URL original como texto.
        app.MapGet("/url", async (
                string uId, 
                IUrlRepository urls,
                CancellationToken cancellationToken) =>
            {
                var link = await urls.GetAsync(uId, cancellationToken);

                return link?.Url;
            })
            .WithName("GetUrl")
            .WithSummary("Retorna a URL original a partir do identificador encurtado.");
    }
}
