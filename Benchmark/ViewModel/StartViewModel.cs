using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Benchmark.Model;
using Benchmark.Service;
using GalaSoft.MvvmLight.Messaging;
using LiveCharts;
using LiveCharts.Wpf;

namespace Benchmark.ViewModel
{
    public class StartViewModel : BaseViewModel, INotifyPropertyChanged 
    {
       
        public DBManager dbManager;
        public ResultsChartViewModel ResultsChartViewModel { get; set; }       
        
        public SeriesCollection Zapytanie1 { get; set; }
        public string[] NazwyBaz { get; set; }
        public Func<double, string> Formatter { get; set; }

       
        private Visibility _visibility1;
        public Visibility Visibility1
        {
            get { return _visibility1; }
            set
            {
                _visibility1 = value;
                RaisePropertyChanged("Visibility1");
            }
        }
      
        private ICommand _start;
        public ICommand Start
        {
            get
            {
                if (_start == null)
                {
                    _start = new RelayCommand(param =>  MetodaStartu(), param => true);
                }
                return _start;
            }
        }

      
        private string _informacja;
        public string Informacja {
            get
            {
                return _informacja;
            }
            set 
            {
                _informacja = value;
                RaisePropertyChanged("Informacja");
            } 
        }

        private double _pasekPostepu;
        public double PasekPostepu
        {
            get
            {
                return _pasekPostepu;
            }
            set
            {
                _pasekPostepu = value;
                RaisePropertyChanged("PasekPostepu");
            }
        }


        private void ProcessMessage(string message)
        {
            Informacja = message;       
        }

        private void ProcessMessage(int message)
        {
            PasekPostepu = (double)message;
        }


        public StartViewModel()
        {           
            Messenger.Default.Register<string>(this, this.ProcessMessage);
            Messenger.Default.Register<int>(this, this.ProcessMessage);
            Visibility1 = Visibility.Hidden;
            dbManager = new DBManager();
        }

        public async void MetodaStartu()
        {
            PasekPostepu = 0;
            Visibility1 = Visibility.Visible;
            if (await dbManager.StartTest()) 
            {
                MessageBoxResult result = MessageBox.Show("Test się zakończył, przejdź do 'Porównanie' aby zobaczyć wyniki",
                                          "Informacja",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Information);
                ResultsChartViewModel = ResultsChartViewModel.LoadViewModel(dbManager.CzasDgraph, dbManager.CzasArango,
                    dbManager.CzasNeo4j, dbManager.CzasOrient);

            }

        }


    }
}
