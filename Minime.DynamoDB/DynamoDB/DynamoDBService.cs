using Amazon.DynamoDBv2.DataModel;
using Minime.Services.DynamoDB.Tables;

namespace Minime.Services.DynamoDB
{
    public class DynamoDbService : IDynamoDBService
    {
        private readonly IDynamoDBContext _context;
        public DynamoDbService(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<MinimeUrlModel> GetLinkAsync(string uid)
        {
            return await _context.LoadAsync<MinimeUrlModel>(uid);
        }

        public async Task<MinimeUrlModel> NewLinkAsync(string url)
        {
            try
            {
                var link = new MinimeUrlModel(url);

                await _context.SaveAsync(link);

                return link;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
