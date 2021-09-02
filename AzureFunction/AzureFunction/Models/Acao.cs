using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunction.Models
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
