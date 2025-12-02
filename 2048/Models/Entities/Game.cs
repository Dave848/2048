using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public bool IsFinished { get; set; } = false;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime? EndTime { get; set; }
        public required ICollection<GameState> GameStates { get; set; }
        public int PlayerId { get; set; }
        public required Player Player { get; set; }
    }
}
