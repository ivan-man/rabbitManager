using System;

namespace ExampleConsoleApp
{
    public class TestDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            var str = $"{(Guid != Guid.Empty ? Guid.ToString() + Environment.NewLine : string.Empty)}{Id}: {Message}";
            return str;
        }
    }
}
