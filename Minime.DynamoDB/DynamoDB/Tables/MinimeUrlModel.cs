using Amazon.DynamoDBv2.DataModel;
using Minime.DynamoDB.Utils;

namespace Minime.Services.DynamoDB.Tables
{
    [DynamoDBTable("MinimeUrls")]
    public class MinimeUrlModel
    {
        public MinimeUrlModel()
        {

        }
        public MinimeUrlModel(string url)
        {
            UId = url.OnlyLettersAndNumbers().GetStringId();
            Url = url;
            DateCreated = DateTime.Now.ToString();
        }

        [DynamoDBHashKey]
        public string UId { get; set; }
        [DynamoDBProperty]
        public string Url { get; set; }
        [DynamoDBProperty]
        public string DateCreated { get; set; }
    }
}
