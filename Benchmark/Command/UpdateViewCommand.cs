using Benchmark.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Benchmark.Command
{
    public class UpdateViewCommand : ICommand
    {

        private MainWindowViewModel viewModel;
        InfoViewModel infoViewModel;
        MainViewModel mainViewModel;
        OProgramieViewModel oProgramieViewModel;
        StartViewModel startViewModel;
        WynikiViewModel wynikiViewModel;
        public UpdateViewCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
            infoViewModel = new InfoViewModel();
            mainViewModel = new MainViewModel();
            oProgramieViewModel = new OProgramieViewModel();
            startViewModel = new StartViewModel();
            wynikiViewModel = new WynikiViewModel();
        }

        public event EventHandler CanExecuteChanged;

        
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter.ToString() == "Info")
            {
                viewModel.SelectedViewModel = infoViewModel;//new InfoViewModel();
            }
            else if (parameter.ToString() == "Main")
            {
                viewModel.SelectedViewModel = mainViewModel;//new MainViewModel();
            }
            else if (parameter.ToString() == "Start")
            {
                viewModel.SelectedViewModel = startViewModel; //new StartViewModel();
            }
            else if (parameter.ToString() == "Wyniki")
            {
                viewModel.SelectedViewModel = wynikiViewModel;//new WynikiViewModel();
            }
            else if (parameter.ToString() == "OProgramie")
            {
                viewModel.SelectedViewModel = oProgramieViewModel;//new OProgramieViewModel();
            }
            else
            {
                viewModel.SelectedViewModel = new MainViewModel();
            }
        }

    }
}

