using Newtonsoft.Json;
using System;

namespace KafkaConsumer.Models
{
    public class Acao
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("cotacao")]
        public double? Cotacao { get; set; }
    }
}
