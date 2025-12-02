using _2048.Commands;
using _2048.Models.DTO;
using _2048.Models.GameLogic;
using _2048.Models.GameLogic.Achievements;
using _2048.Services;
using _2048.Stores;
using _2048.Tools;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D.Converters;
using System.Windows.Threading;

namespace _2048.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly GameEngine _engine;
        private readonly GameService _gameService;
        private readonly PlayerService _playerService;
        private readonly GameStateStore _gameStateStore;
        private readonly AchievementManager _achievementManager;

        private int _gameId;
        private int _score;
        private int _movesCount;
        private bool _checkingAchievements;
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                OnPropertyChanged();
            }
        }

        private DispatcherTimer? _timer;
        private TimeSpan _elapsedTime;
        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set { _elapsedTime = value; OnPropertyChanged(); }
        }
        private bool _showFinishedGamePopup;
        public bool ShowFinishedGamePopup
        {
            get => _showFinishedGamePopup;
            set
            {
                _showFinishedGamePopup = value;
                OnPropertyChanged();
            }
        }
        private string _playerName;
        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TileViewModel> Tiles { get; private set; }
        
        public ICommand? MoveUpCommand { get; private set; }
        public ICommand? MoveDownCommand { get; private set; }
        public ICommand? MoveLeftCommand { get; private set; }
        public ICommand? MoveRightCommand { get; private set; }
        public ICommand? SaveScoreButton { get; private set; }
        public ICommand? CancelButton { get; private set; }
        public ICommand? NavigateMainMenuCommand { get; private set; }
        
        public event EventHandler<AchievementDTO>? AchievementComplete;

        public GameViewModel(
            GameService gameService, 
            PlayerService playerService,
            GameStateStore gameStateStore,
            AchievementManager achievementManager,
            NavigationService<MainMenuViewModel> navigateMainService)
        {
            _playerName = "Guest";
            _movesCount = 0;
            _checkingAchievements = false;

            _gameService = gameService;
            _playerService = playerService;
            _gameStateStore = gameStateStore;
            _gameStateStore.ResetGameState();
            _achievementManager = achievementManager;

            _engine = new GameEngine();

            InitializeGameEngine();
            InitializeCommands(navigateMainService);
            InitializeTimer();

            _engine.GameOver += GameOver;

            Tiles = new ObservableCollection<TileViewModel>();
            InitializeTiles();
        }

        private async void InitializeGameEngine()
        {
            bool loadGame = false;
            if (!await _gameService.IsLastGameFinished())
            {
                MessageBoxResult confirmExit = MessageBox.Show(
                    "Do you want to load last game?",
                    "Load last game?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );
                loadGame = confirmExit == MessageBoxResult.Yes;
            }

            if (loadGame)
            {
                var game = await _gameService.LoadLastGame();
                _gameId = game.Id;

                _movesCount = game.MovesCount;
                Score = game.Score;
                _engine.SetGameState(game.BoardState, game.Score);
                ElapsedTime = TimeSpan.FromSeconds(game.TimeElapsed);
            }
            else
            {
                var game = await _gameService.CreateNewGame();
                _gameId = game.Id;

                _engine.SpawnInitialTiles();
                ElapsedTime = TimeSpan.Zero;
            }

            UpdateGameState();
            SaveGameState();
        }

        private void InitializeCommands(NavigationService<MainMenuViewModel> navigateMainService)
        {
            MoveUpCommand = new RelayCommand(() => MoveTiles(MoveDirection.UP));
            MoveDownCommand = new RelayCommand(() => MoveTiles(MoveDirection.DOWN));
            MoveLeftCommand = new RelayCommand(() => MoveTiles(MoveDirection.LEFT));
            MoveRightCommand = new RelayCommand(() => MoveTiles(MoveDirection.RIGHT));
            SaveScoreButton = new RelayCommand(() => SetPlayerForGame());
            CancelButton = new NavigateCommand<MainMenuViewModel>(navigateMainService);
            NavigateMainMenuCommand = new NavigateCommand<MainMenuViewModel>(navigateMainService);
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += TimerTick;
            
            _timer.Start();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            CheckAchievements();
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1));
        }

        private void InitializeTiles()
        {
            var tiles = _engine.GetTiles();

            for (int i = 0; i < 16; i++)
            {
                Tiles.Add(new TileViewModel { Value = tiles[i] });
            }
        }

        private async void MoveTiles(MoveDirection direction)
        {
            bool wasChange = _engine.MoveTiles(direction);
            if (wasChange)
            {
                SoundManager.PlaySound("Resources/Sounds/MovedTile.mp3");
                IncrementMovesCount();
                UpdateGameState();
                SaveGameState();
                CheckAchievements();
                await UpdateTiles();
            }
        }

        private async Task UpdateTiles()
        {
            var tiles = _engine.GetTiles();

            var newTilePosition = _engine.LastSpawnedTileIndices;
            int newTileIndex = newTilePosition.Item1 * 4 + newTilePosition.Item2;

            var mergedPositions = _engine.LastMergedTileIndices
                .Select(pos => pos.Item1 * 4 + pos.Item2)
                .ToList();

            for (int i = 0; i < 16; i++)
            {
                var tile = Tiles[i];
                tile.Value = tiles[i];

                tile.ResetAnimationFlags();
                tile.JustSpawned = i == newTileIndex;
                tile.JustMerged = mergedPositions.Contains(i);
            }

            await Task.Delay(300);

            foreach (var tile in Tiles)
                tile.ResetAnimationFlags();
        }

        private void SaveGameState()
        {
            _gameService.SaveGameState(_gameId);
        }

        private void UpdateGameState()
        {
            Score = _engine.GetScore();

            _gameStateStore.UpdateGameState(Score, _engine.GetTiles(), ElapsedTime.TotalSeconds, _movesCount);
        }
        
        private void IncrementMovesCount()
        {
            _movesCount += 1;
        }

        private async void CheckAchievements()
        {
            if (_checkingAchievements) return;
            _checkingAchievements = true;

            UpdateGameState();
            var achieved = await _achievementManager.CheckAchievements();

            if (achieved.Count() != 0)
                SoundManager.PlaySound("Resources/Sounds/Achievement.mp3");

            foreach (var achievement in achieved)
            {
                AchievementComplete?.Invoke(this, achievement);
            }
            
            _checkingAchievements = false;
        }

        private async void SetPlayerForGame()
        {
            string playerName = PlayerName != "" ? PlayerName : "Guest";

             var player = await _playerService.GetPlayer(playerName) ?? await _playerService.CreatePlayer(playerName);
             _gameService.SetGamePlayer(_gameId, player);

            ShowFinishedGamePopup = false;
            NavigateMainMenuCommand!.Execute(null);
        }

        private async void GameOver()
        {
            _timer!.Stop();

            UpdateGameState();
            SaveGameState();
            _gameService.SetGameFinished(_gameId);

            await UpdateTiles();
            SoundManager.PlaySound("Resources/Sounds/FinishedGame.mp3");

            await Task.Delay(200);

            ShowFinishedGamePopup = true;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= TimerTick;
                _timer = null;
            }

            _engine.GameOver -= GameOver;
        }
    }
}
