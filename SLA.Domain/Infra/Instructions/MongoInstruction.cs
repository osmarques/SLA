using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLA.Domain.Infra.Instruction
{
    public class MongoInstruction
    {
        public string Collection { get; set; }
        public string Regex {get; set; }
    }
}
