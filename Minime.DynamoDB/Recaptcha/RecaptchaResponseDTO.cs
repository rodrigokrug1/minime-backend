using Newtonsoft.Json;

namespace Minime.Services.Recaptcha
{
    public class RecaptchaResponseDTO
    {
        public bool success { get; set; }
        public float score { get; set; }
        public string action { get; set; }
        public string challenge_ts { get; set; }
        public string hostname { get; set; }

        [JsonProperty("error-codes")]
        public List<string> errorCodes { get; set; }
    }
}
