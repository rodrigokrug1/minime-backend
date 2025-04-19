using Microsoft.AspNetCore.Mvc;
using Minime.DynamoDB.Service;

namespace Minime.Lambda.Get.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly ILogger<UrlController> _logger;
        private readonly IDynamoDBService _service;

        public UrlController(ILogger<UrlController> logger, IDynamoDBService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet(Name = "GetUrl")]
        public async Task<string> Get(string uId)
        {
            var urlModel = await _service.GetLinkAsync(uId);

            return urlModel.Url;
        }
    }
}
