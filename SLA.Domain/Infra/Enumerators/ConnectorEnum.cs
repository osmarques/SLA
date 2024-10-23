using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLA.Domain.Infra.Enumerators
{
    public enum ConnectorEnum: int
    {
        MySQL,
        MongoDB,
        OracleDB,
        PostgreSQL,
        RabbitMQ,
        SQLite,
        SQLServer
    }
}
