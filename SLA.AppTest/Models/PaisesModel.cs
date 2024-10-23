using SLA.Domain.Infra.Attributes;
using SLA.Domain.Infra.Data;

namespace SLA.AppTest.Models
{
    public class PaisesModel : DataModel
    {
        [Core(PrimaryKey = true, Restrict = true)]
        public long codigo { get; set; }
        [Core(Restrict = true)]
        public DateTime created { get; set; }
        [Core(Restrict = true)]
        public DateTime updated { get; set; }
        public long ibge { get; set; }
        public string nome { get; set; }
        public string iso3 { get; set; }
        public string continente { get; set; }
        public string ddi { get; set; }

        public PaisesModel() => SetTable("pai_paises");
    }
}
