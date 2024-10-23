using SLA.Domain.Infra.Enumerators;
using SLA.Domain.Infra.Interfaces;

namespace SLA.Domain.Infra.Data.Filters
{
    public class DataFilterKey : IDataFilter
    {
        public TypeDataFilterEnum Type { get; set; }
        public dynamic Key { get; set; }

        public DataFilterKey() => Type = TypeDataFilterEnum.Key;

        public DataFilterKey(dynamic Key) 
        {
            Type = TypeDataFilterEnum.Key;
            this.Key = Key;
        }
    }
}
