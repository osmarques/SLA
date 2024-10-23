using SLA.Domain.Infra.Enumerators;
using SLA.Domain.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLA.Domain.Infra.Data.Filters
{
    public class DataFilterPage : IDataFilter
    {
        public TypeDataFilterEnum Type { get; set; }
        public int Page { get; set; } = -1;

        public DataFilterPage() => Type = TypeDataFilterEnum.Page;

        public DataFilterPage(int Offset) 
        {
            Type = TypeDataFilterEnum.Page;
            Page = Offset;
        }
    }
}
