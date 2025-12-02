using _2048.Models.DTO;
using _2048.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.GameLogic.Achievements
{
    public class AchievementManager
    {
        private readonly AchievementsService _achievementsService;
        private readonly AchievementCheckerFactory _achievementCheckerFactory;

        public AchievementManager(AchievementsService achievementsService, AchievementCheckerFactory achievementCheckerFactory) 
        {
            _achievementsService = achievementsService;
            _achievementCheckerFactory = achievementCheckerFactory;
        }

        public async Task<IList<AchievementDTO>> CheckAchievements()
        {
            var uncompleted = await _achievementsService.GetUncompleted();
            List<AchievementDTO> completed = [];

            foreach(var achievement in uncompleted)
            {
                var checker = _achievementCheckerFactory.GetChecker(achievement);

                if (checker != null && await checker.IsAchievedAsync(achievement.TargetValue))
                {
                    var completedAchievement = await _achievementsService.SetCompleted(achievement.Id);
                    completed.Add(completedAchievement);
                }
            }

            return completed;
        }
    }
}
