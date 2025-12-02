using _2048.Models.Database;
using _2048.Models.DTO;
using _2048.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Services
{
    public class AchievementsService
    {
        private readonly DatabaseContext _db;

        public AchievementsService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<IList<AchievementDTO>> GetAll()
        {
            return await _db.Achievements
                .Select(a => new AchievementDTO
                {
                    Id = a.Id,
                    TargetValue = a.TargetValue,
                    Name = a.Name,
                    Description = a.Description,
                    IconPath = a.IconPath,
                    IsCompleted = a.IsCompleted,
                    CompletedAt = a.CompletedAt,
                    Type = a.Type
                })
                .ToListAsync();
        }

        public async Task<IList<AchievementDTO>> GetUncompleted()
        {
            return await _db.Achievements
                .Where(a => !a.IsCompleted)
                .Select(a => new AchievementDTO
                {
                    Id = a.Id,
                    TargetValue = a.TargetValue,
                    Name = a.Name,
                    Description = a.Description,
                    IconPath = a.IconPath,
                    IsCompleted = a.IsCompleted,
                    CompletedAt = a.CompletedAt,
                    Type = a.Type
                })
                .ToListAsync();
        }

        public async Task<AchievementDTO> SetCompleted(int id)
        {
            var achievement = await _db.Achievements.Where(a => a.Id == id).FirstOrDefaultAsync()
                ?? throw new InvalidOperationException($"No achievement found for id: {id}.");

            achievement.IsCompleted = true;
            achievement.CompletedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return new AchievementDTO
            {
                Id = achievement.Id,
                TargetValue = achievement.TargetValue,
                Name = achievement.Name,
                Description = achievement.Description,
                IconPath = achievement.IconPath,
                IsCompleted = achievement.IsCompleted,
                CompletedAt = achievement.CompletedAt,
                Type = achievement.Type
            };
        }

        public async Task SetUncompletedAll()
        {
            await _db.Achievements.ForEachAsync(a => a.IsCompleted = false);
            await _db.SaveChangesAsync();
        }
    }
}
