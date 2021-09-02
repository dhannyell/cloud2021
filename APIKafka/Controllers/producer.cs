using APIKafka.Models;
using APIKafka.ViewModel;
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

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] RequestModel model)
        {
            var bootstrapServer = _configuration["bootstrapServer"];
            var topicName = _configuration["topicName"];
            var password = _configuration["password"];

            _logger.LogInformation($"BootstrapServers: {bootstrapServer}");


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

                string outputModel = JsonConvert.SerializeObject(model);

         
                using (var producer = new ProducerBuilder<string, string>(config).Build())
                {
                    var result = await producer.ProduceAsync(
                        topicName, new Message<string, string> { Value = outputModel, Key="1" }
                    );

                    var ViewModel = new RequestViewModel
                    {
                        Name = model.Name,
                        Cotacao = model.Cotacao,
                        Date = model.Date,
                        Partition = result.Partition,
                        Status = result.Status.ToString(),
                        Offset = result.Offset.Value,
                        TopicPartition =  result.TopicPartition.Partition
                    };

                    return Ok(ViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }

            return BadRequest();

        }
    }
}
