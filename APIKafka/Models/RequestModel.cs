using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIKafka.Models
{
    public class RequestModel
    {
        private DateTime _date;

        public RequestModel()
        {
            _date = DateTime.Now;
        }

        [JsonRequired]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("date")]
        public DateTime Date
        {
            get => _date;
        }

        [JsonProperty("cotacao")]
        public double Cotacao { get; set; }
    }
}
