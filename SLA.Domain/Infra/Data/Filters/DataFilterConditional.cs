using SLA.Domain.Infra.Enumerators;
using SLA.Domain.Infra.Interfaces;

namespace SLA.Domain.Infra.Data.Filters
{
    public class DataFilterConditional : IDataFilter
    {
        public TypeDataFilterEnum Type { get; set; }
        public bool AndConditional { get; set; }
    }
}
