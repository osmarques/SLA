using SLA.Domain.Application.Enumerators;
using SLA.Domain.Infra.Attributes;

namespace SLA.Domain.Infra.Data
{
    public class DataModel
    {
        [Core(Ignore = true)]
        private TypeModelEnum _type { get; set; }
        [Core(Ignore = true)]
        private string _table { get; set; }


        [Core(Ignore = true)]
        public TypeModelEnum Type { get => _type; }
        [Core(Ignore = true)]
        public string Table { get => _table; }

        public string SetTable(string Table) => _table = Table;
        public TypeModelEnum SetType(TypeModelEnum Type) => _type = Type;

        public DataModel() { }
        public DataModel(string Table) => _table = Table;
    }
}
