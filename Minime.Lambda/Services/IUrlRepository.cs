using Minime.Lambda.Models;

namespace Minime.Lambda.Services;

public interface IUrlRepository
{
    Task<MinimeUrl?> GetAsync(string uid, CancellationToken cancellationToken = default);

    Task<MinimeUrl> CreateAsync(string url, CancellationToken cancellationToken = default);
}
