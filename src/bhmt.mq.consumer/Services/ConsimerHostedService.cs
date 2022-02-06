using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace bhmt.mq.consumer.Services
{
    public class ConsumerHostedService : BackgroundService
    {
        private ConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;
        private readonly string Host = "localhost";
        private readonly string Q = "helloworld";

        public ConsumerHostedService() { }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            connectionFactory = new ConnectionFactory
            {
                HostName = Host,
                DispatchConsumersAsync = true,
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
            };

            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(Q, true, false, false, null);
            channel.BasicQos(0, 1, false);

            Console.WriteLine($"{Q} listening");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (bc, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var msg = Encoding.UTF8.GetString(body);

                        Console.WriteLine($" [x] Recieved: {msg}");
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        await Task.Delay(0, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        channel.BasicNack(ea.DeliveryTag, false, false);
                        Console.WriteLine(ex.Message);
                    }
                };

                channel.BasicConsume(queue: Q, autoAck: false, consumer: consumer);
                await Task.CompletedTask;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            connection.Close();
            Console.WriteLine("Rabbit MQ connection closed");
        }
    }
}