using _2048.Models.Database;
using _2048.Models.DTO;
using _2048.Models.Entities;
using _2048.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _2048.Services
{
    public class GameService
    {
        private readonly DatabaseContext _db;
        private readonly PlayerService _playerService;
        private readonly GameStateStore _gameStateStore;

        public GameService(DatabaseContext db, PlayerService playerService, GameStateStore gameStateStore)
        {
            _db = db;
            _playerService = playerService;
            _gameStateStore = gameStateStore;
        }

        public async Task<GameDTO> CreateNewGame()
        {
            Player? player = await _playerService.GetGuestPlayer()
                ?? throw new InvalidOperationException("Guest player is null.");

            Game game = new Game
            {
                Player = player,
                GameStates = new List<GameState>()
            };

            _db.Games.Add(game);
            await _db.SaveChangesAsync();

            return new GameDTO { Id = game.Id, BoardState = [] };
        }

        public async Task<Game> GetGameOrThrowAsync(int gameId)
        {
            var game = await _db.Games.FirstOrDefaultAsync(g => g.Id == gameId);

            return game == null ? throw new InvalidOperationException($"No game found with ID {gameId}") : game;
        }

        public async void SaveGameState(int gameId)
        {
            var game = await GetGameOrThrowAsync(gameId);

            var gameState = new GameState
            {
                Game = game,
                Score = _gameStateStore.Score,
                MovesCount = _gameStateStore.MovesCount,
                TimeElapsed = _gameStateStore.TimeElapsed,
                Board = _gameStateStore.SerializeBoard()
            };

            game.GameStates.Add(gameState);
            await _db.SaveChangesAsync();
        }

        public async void SetGameFinished(int gameId)
        {
            var game = await GetGameOrThrowAsync(gameId);

            game.IsFinished = true;
            game.EndTime = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        public async void SetGamePlayer(int gameId, Player player)
        {
            var game = await GetGameOrThrowAsync(gameId);

            game.Player = player;

            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsLastGameFinished()
        {
            var lastGame = await _db.Games.OrderByDescending(g => g.StartTime).FirstOrDefaultAsync();
            return lastGame == null || lastGame.IsFinished;
        }

        public async Task<GameDTO> LoadLastGame()
        {
            var lastGame = await _db.Games
                .Include(g => g.GameStates)
                .OrderByDescending(g => g.StartTime)
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("No saved game found.");

            var gameState = lastGame.GameStates
                .OrderByDescending(bs => bs.SavedDate)
                .FirstOrDefault()
                ?? throw new InvalidOperationException("No board state found for the last game.");

            var board = JsonSerializer.Deserialize<int[]>(gameState.Board)
                ?? throw new InvalidOperationException("Invalid board state.");

            return new GameDTO
            {
                Id = lastGame.Id,
                Score = gameState.Score,
                MovesCount = gameState.MovesCount,
                TimeElapsed = gameState.TimeElapsed,
                BoardState = board
            };
        }
    }
}
