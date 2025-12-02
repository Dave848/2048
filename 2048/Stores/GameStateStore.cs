using _2048.Models.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2048.Stores
{
    public class GameStateStore
    {
        public int Score { get; set; }
        public required int[] Board { get; set; }
        public double TimeElapsed{ get; set; }
        public int MovesCount { get; set; }

        public void UpdateGameState(int score, int[] board, double timeElapsed, int movesCount)
        {
            Score = score;
            Board = board;
            TimeElapsed = timeElapsed;
            MovesCount = movesCount;
        }

        public void ResetGameState()
        {
            Score = 0;
            Board = [];
            TimeElapsed = 0;
            MovesCount = 0;
        }

        public int GetMaxTile()
        {
            return Board.Max();
        }

        public string SerializeBoard()
        {
            return JsonSerializer.Serialize(Board);
        }
    }
}
