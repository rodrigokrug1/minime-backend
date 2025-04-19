using Microsoft.AspNetCore.Mvc;
using Minime.Lambda.DTO;
using Minime.Service.Recaptcha;
using Minime.Services.DynamoDB;

namespace Minime.Lambda.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LinkController : ControllerBase
    {
        private readonly ILogger<LinkController> _logger;
        private readonly IDynamoDBService _dynamo;
        private readonly IRecaptchaService _recaptchaService;


        private readonly string _baseUrl = "minime.cloud";

        public LinkController(ILogger<LinkController> logger, IDynamoDBService dynamo, IRecaptchaService recaptchaService)
        {
            _logger = logger;
            _dynamo = dynamo;
            _recaptchaService = recaptchaService;
        }

        /// <summary>
        /// Generate a new minified url
        /// </summary>
        /// <param name="url">Url to be reduced</param>
        /// <returns>Minified url</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ActionName("NewMinifiedUrl")]
        public async Task<IActionResult> Post([FromBody] NewLinkMinifyRequest request, CancellationToken cancellationToken)
        {
            var isRecaptchaValid = await _recaptchaService.ValidateAsync(cancellationToken);

            if (isRecaptchaValid)
            {
                var newUrl = await _dynamo.NewLinkAsync(request.Url);

                return Created("", new { shortUrl = $"{_baseUrl}/{newUrl.UId}" });
            }

            return StatusCode(429);
        }

        /// <summary>
        /// Retrieve url 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>Redirect to an original url</returns>
        [HttpGet("/" + "{uid}")]     // a barra ignora o nome da rota e vai direto receber o uid na querystring
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetRedirect(string uid)
        {
            var model = await _dynamo.GetLinkAsync(uid);

            if (model is null) return NoContent();

            return Redirect(model.Url);
        }

    }
}
