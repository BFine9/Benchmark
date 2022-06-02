using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmark.Model;
using Benchmark.Service;
using Dgraph;

namespace Benchmark.ViewModel
{
    public class InfoViewModel: BaseViewModel, INotifyCollectionChanged
    {
        public ObservableCollection<BazaModel> ListaBaz { get; set; }
        //public ObservableCollection<ZapytaniaModel> Zapytania { get; set; }
        
        private PobierzInfo pI;

        public InfoViewModel()
        {
            pI = new PobierzInfo(true);           
            ListaBaz = new ObservableCollection<BazaModel>(pI.pobierzBazy());
            //Zapytania = new ObservableCollection<ZapytaniaModel>(pI.pobierzZapytania());        
        }        
    }
}
