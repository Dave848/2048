using _2048.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.GameLogic.Achievements.Checkers
{
    public class TimePlayedAchievementChecker : IAchievementChecker
    {
        private readonly GameStateStore _gameStateStore;
        public TimePlayedAchievementChecker(GameStateStore gameStateStore) 
        {
            _gameStateStore = gameStateStore;
        }

        public Task<bool> IsAchievedAsync(int targetValue)
        {
            return Task.FromResult(_gameStateStore.TimeElapsed >= targetValue);
        }
    }
}
