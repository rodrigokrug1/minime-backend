using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Minime.Lambda.Models;

namespace Minime.Lambda.Services;

public sealed class DynamoDbUrlRepository(IDynamoDBContext context, IAmazonDynamoDB client) : IUrlRepository
{
    private const int MaxAttempts = 5;

    public async Task<MinimeUrl?> GetAsync(string uid, CancellationToken cancellationToken = default)
        => await context.LoadAsync<MinimeUrl>(uid, cancellationToken);

    // Grava com attribute_not_exists(UId): se o UId sorteado já existir, o
    // DynamoDB recusa a escrita em vez de sobrescrever o link anterior.
    // SaveAsync do modelo de alto nível não aceita condição, por isso o PutItem.
    public async Task<MinimeUrl> CreateAsync(string url, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var link = new MinimeUrl(url);

            var request = new PutItemRequest
            {
                TableName = MinimeUrl.TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    [nameof(MinimeUrl.UId)] = new() { S = link.UId },
                    [nameof(MinimeUrl.Url)] = new() { S = link.Url },
                    [nameof(MinimeUrl.DateCreated)] = new() { S = link.DateCreated }
                },
                ConditionExpression = $"attribute_not_exists({nameof(MinimeUrl.UId)})"
            };

            try
            {
                await client.PutItemAsync(request, cancellationToken);

                return link;
            }
            catch (ConditionalCheckFailedException)
            {
                // UId já em uso: sorteia outro na próxima iteração.
            }
        }

        throw new InvalidOperationException(
            $"Não foi possível gerar um UId livre após {MaxAttempts} tentativas.");
    }
}
