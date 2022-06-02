using Benchmark.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Benchmark.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private BaseViewModel _selectedViewModel;
        public BaseViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                RaisePropertyChanged(nameof(SelectedViewModel));
            }
        }

        public ICommand UpdateViewCommand { get; set; }

        public MainWindowViewModel()
        {
            UpdateViewCommand = new UpdateViewCommand(this);
            SelectedViewModel = new MainViewModel();
        }

    }
}
