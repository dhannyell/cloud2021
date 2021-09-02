using APIKafka.Models;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIKafka.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class producerHub : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<producer> _logger;

        static EventHubProducerClient producerClient;

        public producerHub(ILogger<producer> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> post([FromBody] RequestModel model)
        {
            var password = _configuration["password"];

            producerClient = new EventHubProducerClient(password, "acoes");

            var sendOptions = new SendEventOptions
            {
                PartitionId = $"{Convert.ToInt32(model.Particao)}"
            };

            string outputModel = JsonConvert.SerializeObject(model);


            List<EventData> data = new List<EventData>();

            data.Add(new EventData(outputModel));

            await producerClient.SendAsync(data, sendOptions).ConfigureAwait(false);

            return Ok();
        }
    }
}
