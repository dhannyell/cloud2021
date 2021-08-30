using AzureFunction.Data;
using AzureFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
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

            await AcoesRepository.Save(new Acao());
            foreach (KafkaEventData<string> eventData in events)
            {
                log.LogInformation($"C# Kafka trigger function processed a message: {eventData.Value}");
            }
        }
    }
}
