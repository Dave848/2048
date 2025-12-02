using _2048.Models.Database;
using _2048.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Services
{
    public class ScoreboardService
    {
        private readonly DatabaseContext _db;

        public ScoreboardService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<int> GetMaxPageNumber(int pageSize)
        {
            if (pageSize <= 0) throw new ArgumentException("pageSize must be greater than 0");

            var total = await _db.Games.Where(g => g.IsFinished).CountAsync();

            return (int)Math.Ceiling(total / (double)pageSize);
        }

        public async Task<IList<RecordDTO>> GetBestScores(int page, int pageSize)
        {
            var finishedGames = await _db.Games
                .Where(g => g.IsFinished)
                .Include(g => g.Player)
                .Include(g => g.GameStates)
                .ToListAsync();

            var records = finishedGames
                .Select(g => 
                {
                    var state = g.GameStates.OrderByDescending(s => s.SavedDate).FirstOrDefault();
                    return new RecordDTO
                    {
                        PlayerName = g.Player.Name,
                        Score = state?.Score ?? 0,
                        TimeElapsed = state?.TimeElapsed ?? 0,
                        FinishedDate = g.EndTime ?? DateTime.MinValue
                    };
                })
                .OrderByDescending(record => record.Score)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return records;
        }
    }
}
