using SimpleRabbit.Common.Configs;
using SimpleRabbit.Cunsuming;
using SimpleRabbit.Publisher;
using System;
using System.Threading.Tasks;

namespace ExampleConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = new RabbitMqConfig
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                ExchangePrefix = "myTestExchange",
            };

            using var consumeMngr = ConsumeManager.Create(settings);
            consumeMngr.AddConsumer("myTestQueue2", new Action<TestDto>(TestConsumer.ProcessMessage), true);

            consumeMngr.ConnectionRecoveredEvent += RabbitConnectionRecovered;
            consumeMngr.ConnectionShutdownEvent += RabbitConnectionShutdown;


            var pubSrv = PublishService.Create(settings);

            Console.WriteLine("App started");

            var id = 0;
            while (true)
            {
                Console.WriteLine();

                var message = Console.ReadLine();

                if (message == "q" || message == "quit") break;

                var dto = new TestDto
                {
                    Id = ++id,
                    Guid = Guid.NewGuid(),
                    Message = message
                };

                await pubSrv.PublishAsync(dto);

                pubSrv.Publish(dto);
            }

            consumeMngr.ConnectionRecoveredEvent -= RabbitConnectionRecovered;
            consumeMngr.ConnectionShutdownEvent -= RabbitConnectionShutdown;
        }

        private static void RabbitConnectionShutdown(object sender, EventArgs e)
        {
            Console.WriteLine("Connection lost");
        }

        private static void RabbitConnectionRecovered(object sender, EventArgs e)
        {
            Console.WriteLine("Connection recovered");
        }
    }
}
