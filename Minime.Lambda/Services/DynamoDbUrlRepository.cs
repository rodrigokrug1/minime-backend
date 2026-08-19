using Amazon.DynamoDBv2.DataModel;
using Minime.Lambda.Models;

namespace Minime.Lambda.Services;

public sealed class DynamoDbUrlRepository(IDynamoDBContext context) : IUrlRepository
{
    public async Task<MinimeUrl?> GetAsync(string uid, CancellationToken cancellationToken = default)
        => await context.LoadAsync<MinimeUrl>(uid, cancellationToken);

    public async Task<MinimeUrl> CreateAsync(string url, CancellationToken cancellationToken = default)
    {
        var link = new MinimeUrl(url);

        await context.SaveAsync(link, cancellationToken);

        return link;
    }
}
