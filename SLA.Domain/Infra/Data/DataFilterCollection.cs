using SLA.Domain.Infra.Interfaces;

namespace SLA.Domain.Infra.Data
{
    public class DataFilterCollection : IDataFilterCollection
    {
        public List<IDataFilter> Property { get; set; }

        public DataFilterCollection() => Property = new List<IDataFilter>();
    }
}
