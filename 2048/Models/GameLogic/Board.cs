using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2048.Models.GameLogic
{
    public class Board
    {
        private Tile[,] _board { get; } = new Tile[4, 4];

        public Board()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    _board[row, col] = new Tile(row, col);
                }
            }
        }

        public Board(int[] initalBoard)
        {
            if (initalBoard.Length != 16)
                throw new ArgumentException("Board length is other than 16");

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    _board[row, col] = new Tile(initalBoard[row * 4 + col], row, col);
                }
            }
        }

        public IList<Tile> GetEmptyTiles()
        {
            return _board.Cast<Tile>().Where(t => t.Value == 0).ToList();
        }

        public Tile[,] GetTiles()
        {
            return _board;
        }

        public Tile GetTile(int row, int col)
        {
            if(row < 0 || row >= 4 || col < 0 || col >= 4)
                throw new ArgumentOutOfRangeException("Row and Column must be between 0 and 3.");
       
            return _board[row, col];
        }

        public bool SetRow(int row, IList<Tile> line)
        {
            bool changed = false;
            for (int col = 0; col < 4; col++)
            {
                if (_board[row, col].Value != line[col].Value)
                {
                    _board[row, col].Value = line[col].Value;
                    changed = true;
                }
            }
            return changed;
        }

        public bool SetColumn(int col, IList<Tile> line)
        {
            bool changed = false;
            for (int row = 0; row < 4; row++)
            {
                if (_board[row, col].Value != line[row].Value)
                {
                    _board[row, col].Value = line[row].Value;
                    changed = true;
                }
            }
            return changed;
        }
    }
}
