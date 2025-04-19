using Microsoft.AspNetCore.Http;
using Minime.Services.Recaptcha;
using Newtonsoft.Json;

namespace Minime.Service.Recaptcha
{
    public class RecaptchaService : IRecaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _secret = string.Empty;

        public RecaptchaService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _secret = Environment.GetEnvironmentVariable("GOOGLE_RECAPTCHA_SECRET");
        }

        public async Task<bool> ValidateAsync(CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .FirstOrDefault()
                .Replace("Bearer ", "")
                .Trim();

            if (token is null) return default;

            var response = await _httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_secret}&response={token}", null, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var recaptchaResponse = JsonConvert.DeserializeObject<RecaptchaResponseDTO>(response.Content.ReadAsStringAsync(cancellationToken).Result);

                return recaptchaResponse.success;
            }

            return false;
        }
    }
}
