using _2048.Commands;
using _2048.Models;
using _2048.Services;
using _2048.Stores;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace _2048.ViewModels
{
    public record AchievementRecord
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string IconPath { get; set; }
        public required string CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class AchievementsViewModel : ViewModelBase
    {
        private readonly AchievementsService _achievementsService;
        
        private bool _hideCompleted;
        public bool HideCompleted
        {
            get => _hideCompleted;
            set
            {
                if (_hideCompleted != value)
                {
                    _hideCompleted = value;
                    OnPropertyChanged();
                    FilterAndSortAchievements();
                }
            }
        }

        private bool _hideUncompleted;
        public bool HideUncompleted
        {
            get => _hideUncompleted;
            set
            {
                if (_hideUncompleted != value)
                {
                    _hideUncompleted = value;
                    OnPropertyChanged();
                    FilterAndSortAchievements();
                }
            }
        }

        private bool _sortAlphabetically;
        public bool SortAlphabetically
        {
            get => _sortAlphabetically;
            set
            {
                if (_sortAlphabetically != value)
                {
                    _sortAlphabetically = value;
                    OnPropertyChanged();
                    FilterAndSortAchievements();
                }
            }
        }

        private bool _sortByDate;
        public bool SortByDate
        {
            get => _sortByDate;
            set
            {
                if (_sortByDate != value)
                {
                    _sortByDate = value;
                    OnPropertyChanged();
                    FilterAndSortAchievements();
                }
            }
        }

        public ObservableCollection<AchievementRecord> AllAchievements { get; set; }
        public ObservableCollection<AchievementRecord> Achievements { get; set; }
        public ICommand NavigateMainMenuCommand { get; }
        public ICommand ResetAchievementsCommand{ get; }


        public AchievementsViewModel(NavigationService<MainMenuViewModel> navigateMainService, AchievementsService achievementsService)
        {
            _achievementsService = achievementsService;

            NavigateMainMenuCommand = new NavigateCommand<MainMenuViewModel>(navigateMainService);
            ResetAchievementsCommand = new RelayCommand(ResetAchievements);

            AllAchievements = [];
            Achievements = []; 
        }

        public async Task Initialize()
        {
            await LoadAchievements();
            foreach (var item in AllAchievements)
                Achievements.Add(item);
        }

        private async Task LoadAchievements() 
        {
            AllAchievements.Clear();

            foreach(var achievement in await _achievementsService.GetAll())
            {
                AllAchievements.Add(new AchievementRecord
                {
                    Name = achievement.Name,
                    Description = achievement.Description,
                    CompletedAt = achievement.CompletedAt?.ToString() ?? "Not completed",
                    IconPath = achievement.IconPath,
                    IsCompleted = achievement.IsCompleted
                });
            }
        }

        private void FilterAndSortAchievements()
        {
            var filtered = AllAchievements.AsEnumerable();

            if (HideCompleted)
                filtered = filtered.Where(a => !a.IsCompleted);

            if (HideUncompleted)
                filtered = filtered.Where(a => a.IsCompleted);

            if (SortAlphabetically)
                filtered = filtered.OrderBy(a => a.Name);

            if (SortByDate)
                filtered = filtered.OrderBy(a => a.CompletedAt);

            Achievements.Clear();
            foreach (var item in filtered)
                Achievements.Add(item);
        }

        private async void ResetAchievements()
        {
            MessageBoxResult confirmExit = MessageBox.Show(
                "Do you want to reset your achievements? All achievement progress will be reseted",
                "Reset Achievements",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirmExit != MessageBoxResult.Yes)
                return;

            await _achievementsService.SetUncompletedAll();

            await LoadAchievements();
            FilterAndSortAchievements();
        }
    }
}
