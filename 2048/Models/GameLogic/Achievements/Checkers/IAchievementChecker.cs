using _2048.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.GameLogic.Achievements.Checkers
{
    public interface IAchievementChecker
    {
        Task<bool> IsAchievedAsync(int targetValue);
    }
}
