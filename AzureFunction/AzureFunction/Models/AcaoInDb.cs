using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace AzureFunction.Models
{
    public class AcaoInDb
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Date { get; set; }

        [BsonElement("cotacao")]
        public double? Cotacao { get; set; }
    }
}
