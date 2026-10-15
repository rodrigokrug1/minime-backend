using System.Security.Cryptography;
using Amazon.DynamoDBv2.DataModel;

namespace Minime.Lambda.Models;

[DynamoDBTable(TableName)]
public class MinimeUrl
{
    public const string TableName = "MinimeUrls";

    private const int UidLength = 6;

    // Alfabeto fixo, sem caracteres ambíguos (0/O, 1/l/I), para que o link
    // continue legível quando alguém o digita a partir de algo impresso.
    private const string UidAlphabet = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public MinimeUrl()
    {
    }

    public MinimeUrl(string url)
    {
        UId = CreateUid();
        Url = url;
        DateCreated = DateTimeOffset.UtcNow.ToString("O");
    }

    [DynamoDBHashKey]
    public string UId { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string DateCreated { get; set; } = string.Empty;

    // Sorteia do alfabeto completo, nunca dos caracteres da URL de origem:
    // o pool antigo dependia da entrada e colapsava para pouquíssimas
    // combinações em URLs curtas. RandomNumberGenerator em vez de Random
    // porque um UId previsível permitiria adivinhar links de terceiros.
    private static string CreateUid()
        => new(RandomNumberGenerator.GetItems<char>(UidAlphabet, UidLength));
}
