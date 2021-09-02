using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using AzureFunction.Data;
using AzureFunction.Models;
using Flurl;
using Flurl.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunction
{
    public class KafkaTrigger
    {
        // KafkaTrigger sample 
        // Consume the message from "topic" on the LocalBroker.
        // Add `BrokerList` and `Password` to the local.settings.json
        // For EventHubs
        // "BrokerList": "{EVENT_HUBS_NAMESPACE}.servicebus.windows.net:9093"
        // "Password":"{EVENT_HUBS_CONNECTION_STRING}
        [FunctionName("KafkaTrigger")]
        public static async Task RunAsync(
            [KafkaTrigger("%BrokerList%",
                          "%Topic%",
                          Username = "$ConnectionString",
                          Password = "%Password%",
                          Protocol = BrokerProtocol.SaslSsl,
                          AuthenticationMode = BrokerAuthenticationMode.Plain,
                          ConsumerGroup = "$Default")] KafkaEventData<string>[] events, ILogger log)
        {
            
            foreach (KafkaEventData<string> eventData in events)
            {
                EventHubProducerClient producerClient;

                Acao acao = JsonConvert.DeserializeObject<Acao>(eventData.Value);

                var response = await $"https://api.hgbrasil.com/".AppendPathSegment("finance/stock_price")
                    .SetQueryParams(new { key = "79d3c293", symbol = acao.Name }).GetJsonAsync<dynamic>().ConfigureAwait(false);

                var name = acao.Name.ToUpper();


                var results = response.results;
                var acaoCompany = results.SelectToken($"{name}");
                double? price = acaoCompany?.SelectToken("price");

                if (price == null)
                    price = 0;

                acao.Cotacao = price;

                await AcoesRepository.Save(acao);

                var password = Environment.GetEnvironmentVariable("Password");
                var topicName = Environment.GetEnvironmentVariable("Topic");

                producerClient = new EventHubProducerClient(password, topicName);

                int partion = 1;

                if (price == 0)
                    partion = 2;

                var sendOptions = new SendEventOptions
                {
                    PartitionId = $"{partion}"
                };

                string outputModel = JsonConvert.SerializeObject(acao);

                List<EventData> data = new List<EventData>();

                data.Add(new EventData(outputModel));

                await producerClient.SendAsync(data, sendOptions).ConfigureAwait(false);
                log.LogInformation($"Data Send to partition: {partion}");


                log.LogInformation($"C# Kafka trigger function processed a message: {eventData.Value} Partition: {eventData.Partition}");
            }
        }
    }
}
