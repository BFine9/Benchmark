using ArangoDBNetStandard.GraphApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Model
{
    public class CityModelDistance
    {
        public long IdFirst { get; set; }
        public long IdSecond { get; set; }
        //public string CityFirst { get; set; } mam idki
        //public string CitySecond { get; set; }
        public int Distance { get; set; }
    }
    public class CityModelDistanceDgraph
    {
        public long IdSecond { get; set; }
        //public string Uid { get; set; }
        public int Distance { get; set; }
    }
    public class CityModelDistanceArango : PostEdgeDefinitionBody
    {
        public int Distance { get; set; }
    }
}
