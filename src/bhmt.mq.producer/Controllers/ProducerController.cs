using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Text;

namespace bhmt.mq.producer.Conrtrollers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly string Host = "localhost";
        private readonly string Q = "helloworld";

        public ProducerController() { }

        [HttpPost]
        public IActionResult Send()
        {
            var factory = new ConnectionFactory
            {
                HostName = Host,
                RequestedHeartbeat = TimeSpan.FromSeconds(60)
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(Q, true, false, false, null);
                var body = Encoding.UTF8.GetBytes("Hello World!");

                channel.BasicPublish("", Q, null, body);
                return Ok();
            }
        }
    }
}