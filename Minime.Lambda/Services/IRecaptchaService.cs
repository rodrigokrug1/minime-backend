namespace Minime.Lambda.Services;

public interface IRecaptchaService
{
    Task<bool> ValidateAsync(string? token, CancellationToken cancellationToken = default);
}
