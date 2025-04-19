namespace Minime.Service.Recaptcha
{
    public interface IRecaptchaService
    {
        Task<bool> ValidateAsync(CancellationToken cancellationToken);
    }
}
