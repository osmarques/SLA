using SLA.Domain.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLA.AppTest.Models
{
    public class PositionModel 
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double altitude { get; set; }
    }

    public class LocationModel : DataModel
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string etiqueta { get; set; }
        public PositionModel position { get; set; }
        public LocationModel() => SetTable("locations");
    }
}
