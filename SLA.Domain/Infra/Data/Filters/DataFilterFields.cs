using SLA.Domain.Infra.Enumerators;
using SLA.Domain.Infra.Interfaces;

namespace SLA.Domain.Infra.Data.Filters
{
    public class DataFilterFields : IDataFilter
    {
        public TypeDataFilterEnum Type { get; set; } = TypeDataFilterEnum.Filter;
        public List<FieldsFilter> Fields { get; set; } = Activator.CreateInstance<List<FieldsFilter>>();
    }
}
