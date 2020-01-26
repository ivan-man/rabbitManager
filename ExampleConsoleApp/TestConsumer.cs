using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleConsoleApp
{
    public class TestConsumer
    {
        internal static void ProcessMessage(TestDto message)
        {
            Console.WriteLine(message.ToString());
        }
    }
}
