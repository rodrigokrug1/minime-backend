using System.Text.RegularExpressions;
using Amazon.DynamoDBv2.DataModel;

namespace Minime.Lambda.Models;

[DynamoDBTable("MinimeUrls")]
public partial class MinimeUrl
{
    private const int UidLength = 6;

    public MinimeUrl()
    {
    }

    public MinimeUrl(string url)
    {
        UId = CreateUid(url);
        Url = url;
        DateCreated = DateTime.Now.ToString();
    }

    [DynamoDBHashKey]
    public string UId { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string DateCreated { get; set; } = string.Empty;

    // Estratégia original preservada: sorteia 6 caracteres alfanuméricos da própria URL.
    private static string CreateUid(string url)
    {
        var pool = NonAlphanumericRegex().Replace(url, "");

        return new string([.. Enumerable.Range(0, UidLength).Select(_ => pool[Random.Shared.Next(pool.Length)])]);
    }

    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex NonAlphanumericRegex();
}
