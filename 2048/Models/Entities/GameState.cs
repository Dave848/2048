using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.Entities
{
    public class GameState
    {
        public int Id { get; set; }
        public int Score { get; set; } = 0;
        public int MovesCount { get; set; } = 0;
        public double TimeElapsed { get; set; } = 0.0;
        public required string Board { get; set; }
        public int GameId { get; set; }
        public required Game Game { get; set; }
        public DateTime SavedDate { get; set; } = DateTime.Now;
    }
}
