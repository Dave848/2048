using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Models.GameLogic
{
    public class Tile
    {
        private int _value;
        public int Value
        {
            get => _value;
            set => _value = value;
        }

        private bool _merged;

        public bool Merged
        {
            get => _merged;
            set => _merged = value;
        }

        private readonly int _row;
        private readonly int _col;

        public Tile(int row, int col)
        {
            Value = 0;
            _row = row;
            _col = col;
        }

        public Tile(int initalValue, int row, int col)
        {
            Value = initalValue;
            _row = row;
            _col = col;
        }

        public (int, int) GetPosition()
        {
            return (_row, _col);
        }
    }
}
