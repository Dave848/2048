using _2048.Models.DTO;
using _2048.Models.Entities;
using _2048.Models.GameLogic.Achievements.Checkers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.GameLogic.Achievements
{
    public class AchievementCheckerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AchievementCheckerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAchievementChecker? GetChecker(AchievementDTO achievement)
        {
            return achievement.Type switch
            {
                AchievementType.ReachTile => _serviceProvider.GetService<ReachTileAchievementChecker>(),
                AchievementType.MovesCount => _serviceProvider.GetService<MovesCountAchievementChecker>(),
                AchievementType.TimePlayed => _serviceProvider.GetService<TimePlayedAchievementChecker>(),
                AchievementType.ReachTileWithinMinute => _serviceProvider.GetService<ReachTileWithinMinuteAchievementChecker>(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
