using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIKafka.Models
{
    public class RequestModel
    {
        private readonly DateTime _date;
        private string _name;

        public RequestModel()
        {
            _date = DateTime.Now;
        }

        [JsonRequired]
        [JsonProperty("name")]
        public string Name 
        {
            get => _name;
            set => _name = value.ToLower();
        }

        [JsonProperty("date")]
        public DateTime Date
        {
            get => _date;
        }

        [JsonProperty("cotacao")]
        public double Cotacao { get; set; }

        [JsonIgnore]
        public Particao Particao { get; set; }
    }
}
