using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Model
{
    public class CzasModel 
    {
        public int NrZapytania { get; set; }
        public List<double> CzasZapytania { get; set; } = new List<double>();
        public double AvgCzasZapytania { get; set; }
    }
  
}
