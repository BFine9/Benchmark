using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Model
{
    public class ZapytaniaModel
    {
        public string Tresc { get; set; }
        public string NazwaBazy { get; set; }
    }
    public class RootZapytania
    {
        public List<ZapytaniaModel> Zapytania { get; set; }
    }
}
