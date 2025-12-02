using _2048.Commands;
using _2048.Models;
using _2048.Services;
using _2048.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace _2048.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        public ICommand NavigateGameCommand { get; }
        public ICommand NavigateScoreboardCommand { get; }
        public ICommand NavigateAchievementsCommand { get; }
        public ICommand NavigateAboutCommand { get; }
        public ICommand ExitCommand { get; }


        public MainMenuViewModel(
            NavigationService<GameViewModel> navigateGameService,
            NavigationService<ScoreboardViewModel> navigateScoreboardService,
            NavigationService<AchievementsViewModel> navigateSettingsService,
            NavigationService<AboutViewModel> navigateAboutService
            )
        {
            NavigateGameCommand = new NavigateCommand<GameViewModel>(navigateGameService);
            NavigateScoreboardCommand = new NavigateCommand<ScoreboardViewModel>(navigateScoreboardService);
            NavigateAchievementsCommand = new NavigateCommand<AchievementsViewModel>(navigateSettingsService);
            NavigateAboutCommand = new NavigateCommand<AboutViewModel>(navigateAboutService);

            ExitCommand = new RelayCommand(() => 
            {
                Application.Current.MainWindow.Close();
            });
            
        }
    }
}
