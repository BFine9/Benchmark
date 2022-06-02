using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Benchmark.ViewModel
{
    public class WynikiViewModel : BaseViewModel, INotifyCollectionChanged
    {
        public ObservableCollection<string> ListaWnioskow { get; set; }
        /*  { 
              get { return listaWnioskow; }
              set
              {
                  listaWnioskow = value;
                  FirePropertyChanged("ListaWnioskow");
              }

          }
        */
          private Visibility _visibility;
        public Visibility Visibility1
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                RaisePropertyChanged("Visibility1");
            }
        }
        private Visibility _visibility2;
        public Visibility Visibility2
        {
            get { return _visibility2; }
            set
            {
                _visibility2 = value;
                RaisePropertyChanged("Visibility2");
            }
        }

        public WynikiViewModel()
        {
            Messenger.Default.Register<ObservableCollection<string>>(this, this.ProcessMessage);
            Messenger.Default.Register<bool>(this, this.ProcessMessage2);


            ListaWnioskow = new ObservableCollection<string>();
            Visibility1 = Visibility.Hidden;
            Visibility2 = Visibility.Visible;             
        }

        private void ProcessMessage2(bool obj)
        {
            if(obj)
            {
                Visibility1 = Visibility.Visible;
                Visibility2 = Visibility.Hidden;
            }
        }

        private void ProcessMessage(ObservableCollection<string> obj)
        {
            ListaWnioskow = new ObservableCollection<string>(obj);
        }
    }
}