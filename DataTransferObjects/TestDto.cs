using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects
{
    public class TestDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            var str = $"{(Guid != Guid.Empty ? Guid.ToString() + Environment.NewLine : string.Empty)}";
            str += $"{Id}: {Message}";

            return str;
        }
    }
}
