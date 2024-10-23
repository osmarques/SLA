using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLA.Domain.Infra.Interfaces
{
    public interface IDBConnectionModel
    {
        public IConnectionSettings Connection { get; set; }
    }
}
