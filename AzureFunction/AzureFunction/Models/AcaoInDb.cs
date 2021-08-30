using Newtonsoft.Json;

namespace AzureFunction.Models
{
    public class AcaoInDb
    {
        [JsonProperty(PropertyName = "codigo")]
        public string Codigo { get; set; }
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
        [JsonProperty(PropertyName = "valor")]
        public double Valor {get; set; }
    }
}
