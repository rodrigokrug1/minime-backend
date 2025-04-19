using Minime.Services.DynamoDB.Tables;

namespace Minime.Services.DynamoDB
{
    public interface IDynamoDBService
    {
        Task<MinimeUrlModel> NewLinkAsync(string url);
        Task<MinimeUrlModel> GetLinkAsync(string uid);
    }
}
