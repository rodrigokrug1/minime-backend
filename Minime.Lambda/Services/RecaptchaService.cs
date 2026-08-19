using System.Text.Json.Serialization;

namespace Minime.Lambda.Services;

public sealed class RecaptchaService(HttpClient httpClient, IConfiguration configuration) : IRecaptchaService
{
    private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    private readonly string? _secret = configuration["GOOGLE_RECAPTCHA_SECRET"];

    public async Task<bool> ValidateAsync(string? token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(_secret))
            return false;

        using var response = await httpClient.PostAsync(
            $"{VerifyUrl}?secret={_secret}&response={Uri.EscapeDataString(token)}",
            content: null,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<RecaptchaResponse>(cancellationToken);

        return result?.Success ?? false;
    }
}

public sealed class RecaptchaResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("error-codes")]
    public List<string>? ErrorCodes { get; set; }
}
