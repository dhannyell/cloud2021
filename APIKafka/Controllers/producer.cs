using APIKafka.Models;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace APIKafka.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class producer : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<producer> _logger;

        public producer(ILogger<producer> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] RequestModel model)
        {
            var bootstrapServer = _configuration["bootstrapServer"];
            var topicName = _configuration["topicName"];
            var password = _configuration["password"];

            _logger.LogInformation($"BootstrapServers: {bootstrapServer}");


            var jsonSerializerConfig = new JsonSerializerConfig
            {
                BufferBytes = 100
            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                Url = ""
            };

            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = bootstrapServer,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = "$ConnectionString",
                    SaslPassword = password
                };

                string output = JsonConvert.SerializeObject(model); // {"ExpiryDate":new Date(1230375600000),"Price":0}

                var kek = JsonConvert.DeserializeObject<RequestModel>(output);

                //using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
                using (var producer = new ProducerBuilder<Null, RequestModel>(config)
                    .SetValueSerializer(new JsonSerializer<RequestModel>(null, jsonSerializerConfig)).Build())
                {
                    var result = await producer.ProduceAsync(
                        topicName, new Message<Null, RequestModel> { Value = model }
                    ).ContinueWith(task => task.IsFaulted
                            ? $"error producing message: {task.Exception.Message}"
                            : $"produced to: {task.Result.TopicPartitionOffset}");

                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }

            return Ok();

        }
    }
}
