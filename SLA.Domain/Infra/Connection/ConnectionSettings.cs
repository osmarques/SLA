using SLA.Domain.Infra.Interfaces;

namespace SLA.Domain.Infra.Connection
{
    public class ConnectionSettings : IConnectionSettings
    {
        public string HostType { get; set; } = string.Empty;
        public string Host { get; set; } = "localhost";
        public string Port { get; set; } = string.Empty;
        public string DataBase { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Timeout { get; set; }
        public bool UseTransaction { get; set; }        
    }
}
