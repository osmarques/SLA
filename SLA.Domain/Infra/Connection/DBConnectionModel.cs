using SLA.Domain.Infra.Enumerators;
using SLA.Domain.Infra.Interfaces;
using System.Text.Json.Serialization;

namespace SLA.Domain.Infra.Connection
{
    public class DBConnectionModel
    {        
        public ConnectorEnum Type { get; set; }
        public ConnectionSettings Connection { get; set; }

        public DBConnectionModel() 
        { 
            Connection = new ConnectionSettings();
        }
    }
}
