using SLA.Domain.Infra.Enumerators;
using SLA.Domain.Infra.Interfaces;

namespace SLA.Domain.Infra.Data.Filters
{
    public class DataFilterLimit : IDataFilter
    {
        public TypeDataFilterEnum Type { get; set; }
        public int Limit { get; set; } = -1;

        public DataFilterLimit() => Type = TypeDataFilterEnum.Limit;

        public DataFilterLimit(int Limit)
        {
            Type = TypeDataFilterEnum.Limit;
            this.Limit = Limit;
        }

    }
}
