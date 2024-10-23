namespace SLA.Domain.Infra.Interfaces
{
    public interface IConnectionSettings 
    {
        public string HostType { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string DataBase { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
        public bool UseTransaction { get; set; }
    }
}
