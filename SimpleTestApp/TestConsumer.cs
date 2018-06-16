using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTestApp
{
    public class TestConsumer
    {
        internal static void ProcessMessage(TestDto message)
        {
            Console.WriteLine(message.ToString());
        }
    }
}
