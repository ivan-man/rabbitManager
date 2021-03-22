using RabbitMqManager;
using System;

namespace ExampleConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using IQueueManager mqMngr = QueueManager.CreateManager(new Action<IQueueManager>((mngr) =>
            {
                mngr.SetUserName("guest");
                mngr.SetPassword("guest");
                mngr.SetHostName("localhost");
                mngr.SetPort(5672);
                mngr.SetVirtualHost("/");

                mngr.SetExchangePrefix("myTestExchange");

                mngr.AddConsumer("myTestQueue", new Action<TestDto>(TestConsumer.ProcessMessage), true);

                mngr.ConnectionRecoveredEvent += RabbitConnectionRecovered;
                mngr.ConnectionShutdownEvent += RabbitConnectionShutdown;
            }));

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

                mqMngr.PushMessageAsync(dto);
            }

            mqMngr.ConnectionRecoveredEvent -= RabbitConnectionRecovered;
            mqMngr.ConnectionShutdownEvent -= RabbitConnectionShutdown;
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
