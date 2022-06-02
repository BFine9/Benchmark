using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Model
{
    public class BazaModel
    {
        public string Nazwa { get; set; }
        public string Typ { get; set; }
        public string Opis { get; set; }
        public string URL { get; set; }         
    }

    public class RootBazy
    {
        public List<BazaModel> Bazy { get; set; }
    }
}
