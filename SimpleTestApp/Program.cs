using DataTransferObjects;
using RabbitMqManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IQueueManager mqMngr = QueueManager.CreateManager(new Action<IQueueManager>((mngr) =>
            {
                mngr.SetUserName("guest");
                mngr.SetPassword("guest");
                mngr.SetHostName("localhost");
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

                mqMngr.PushMessage(dto);
            }

            mqMngr.ConnectionRecoveredEvent -= RabbitConnectionRecovered;
            mqMngr.ConnectionShutdownEvent -= RabbitConnectionShutdown;
            mqMngr.Disable();
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
