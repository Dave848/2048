using _2048.Commands;
using _2048.Models;
using _2048.Models.DTO;
using _2048.Services;
using _2048.Stores;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace _2048.ViewModels
{
    public record GameRecord
    {
        public required string PlayerName { get; set; }
        public required int Score { get; set; }
        public required string Time { get; set; }
        public required string Date { get; set; }
    }

    public class ScoreboardViewModel : ViewModelBase
    {
        private readonly ScoreboardService _scoreboardService;
        private readonly int _recordsPerPage = 12;
        private int _maxPageNumber;
        private int _page = 1;
        public int Page
        {
            get { return _page; }
            set
            {
                _page = value;
                OnPropertyChanged(nameof(Page));
            }
        }
        public ObservableCollection<GameRecord> Records { get; set; }
        public ICommand NavigateMainMenuCommand { get; }
        public ICommand PageNext { get; }
        public ICommand PagePrevious { get; }

        public ScoreboardViewModel(NavigationService<MainMenuViewModel> navigateMainService, ScoreboardService scoreboardService)
        {
            _scoreboardService = scoreboardService;
            Records = new ObservableCollection<GameRecord>();

            NavigateMainMenuCommand = new NavigateCommand<MainMenuViewModel>(navigateMainService);
            PagePrevious = new RelayCommand(PagePreviousFunction);
            PageNext = new RelayCommand(PageNextFunction);

            SetMaxPageNumber();
            LoadScoreboard();
        }

        private async void SetMaxPageNumber()
        {
            _maxPageNumber = await _scoreboardService.GetMaxPageNumber(_recordsPerPage);
        }

        private async void LoadScoreboard()
        {
            Records.Clear();
            
            var records = await _scoreboardService.GetBestScores(Page, _recordsPerPage);
            foreach (var record in records)
            {
                Records.Add(new GameRecord
                {
                    PlayerName = record.PlayerName,
                    Score = record.Score,
                    Time = TimeSpan.FromSeconds(record.TimeElapsed).ToString(),
                    Date = record.FinishedDate.ToShortDateString()
                });
            }
        }

        private void PagePreviousFunction()
        {
            if (Page == 1)
                return;

            Page -= 1;
            LoadScoreboard();
        }

        private void PageNextFunction()
        {
            if (Page >= _maxPageNumber)
                return;
                
            Page += 1;
            LoadScoreboard();
        }
    }
}
