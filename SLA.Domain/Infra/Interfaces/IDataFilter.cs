using SLA.Domain.Infra.Enumerators;

namespace SLA.Domain.Infra.Interfaces
{
    public interface IDataFilter
    {
        public TypeDataFilterEnum Type { get; set; }
    }
}
