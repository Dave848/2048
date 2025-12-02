using _2048.Commands;
using _2048.Models;
using _2048.Services;
using _2048.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _2048.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public ICommand NavigateMainMenuCommand { get; }

        public AboutViewModel(NavigationService<MainMenuViewModel> navigateMainService)
        {
            NavigateMainMenuCommand = new NavigateCommand<MainMenuViewModel>(navigateMainService);
        }
    }
}
