using Benchmark.Model;
using LiveCharts;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Benchmark.ViewModel
{
    public class ResultsChartViewModel : INotifyPropertyChanged
    {
        public ChartValues<double> RezultatyKw1 { get; set; }
        public ChartValues<double> RezultatyKw2 { get; set; }
        public ChartValues<double> RezultatyKw3 { get; set; }
        public ChartValues<double> RezultatyKw4 { get; set; }
        public ChartValues<double> RezultatyKw5 { get; set; }

        public Visibility Visibility { get; set; }
     
      
        
        public string[] NazwyBaz { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public static ResultsChartViewModel LoadViewModel(List<CzasModel> CzasDgraph, List<CzasModel> CzasArango,
            List<CzasModel> CzasNeo4j, List<CzasModel> CzasOrient, Action<Task> onLoaded = null)
        {
            ResultsChartViewModel viewmodel = new ResultsChartViewModel();
            viewmodel.Load(CzasDgraph, CzasArango, CzasNeo4j, CzasOrient).ContinueWith(t => onLoaded?.Invoke(t));

            return viewmodel;
        }

        public async Task Load(List<CzasModel> CzasDgraph, List<CzasModel> CzasArango,
            List<CzasModel> CzasNeo4j, List<CzasModel> CzasOrient)
        {
            //RezultatyKw1 = new ChartValues<double> { 10, 4, 2, 55 };
            //RezultatyKw2 = new ChartValues<double> { 12, 123, 123, 1 };
            //RezultatyKw3 = new ChartValues<double> { 4, 4, 4, 4 };
            //RezultatyKw4 = new ChartValues<double> { 5, 34, 34, 1 };
            //RezultatyKw5 = new ChartValues<double> { 11, 133, 44, 1 };
            Visibility = Visibility.Visible;
            RezultatyKw1 = new ChartValues<double> { CzasDgraph[0].AvgCzasZapytania, CzasArango[0].AvgCzasZapytania,
            CzasNeo4j[0].AvgCzasZapytania, CzasOrient[0].AvgCzasZapytania};
            RezultatyKw2 = new ChartValues<double> { CzasDgraph[1].AvgCzasZapytania, CzasArango[1].AvgCzasZapytania,
            CzasNeo4j[1].AvgCzasZapytania, CzasOrient[1].AvgCzasZapytania};
            RezultatyKw3 = new ChartValues<double> { CzasDgraph[2].AvgCzasZapytania, CzasArango[2].AvgCzasZapytania,
            CzasNeo4j[2].AvgCzasZapytania, CzasOrient[2].AvgCzasZapytania};
            RezultatyKw4 = new ChartValues<double> { CzasDgraph[3].AvgCzasZapytania, CzasArango[3].AvgCzasZapytania,
            CzasNeo4j[3].AvgCzasZapytania, CzasOrient[3].AvgCzasZapytania};
            RezultatyKw5 = new ChartValues<double> { CzasDgraph[4].AvgCzasZapytania, CzasArango[4].AvgCzasZapytania,
            CzasNeo4j[4].AvgCzasZapytania, CzasOrient[4].AvgCzasZapytania};

            NazwyBaz = new[] { "DGraph", "ArangoDB", "Neo4j", "OrientDB" };
        }
    }
}
