using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace _2048.Models.GameLogic
{
    public class GameEngine
    {
        private Board _board;
        private int _score;
        private static readonly Random _random = new Random();
        public event Action? GameOver;
        public (int, int) LastSpawnedTileIndices { get; set; }
        public List<(int, int)> LastMergedTileIndices { get; set; } = new List<(int, int)>();

        public GameEngine()
        {
            _board = new Board();
            _score = 0;
            LastSpawnedTileIndices = (-1, -1);

        }

        public void SetGameState(int[] initialBoard, int score)
        {
            _board = new Board(initialBoard);
            _score = score;
        }

        public void SpawnInitialTiles()
        {
            SpawnTile();
            SpawnTile();
            UpdateScore();
        }

        public int[] GetTiles()
        {
            var tiles = _board.GetTiles();
            int[] result = new int[4 * 4];
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    result[row * 4 + col] = tiles[row, col].Value;
                }
            }
            return result;
        }

        public int GetScore()
        {
            return _score;
        }

        private void SpawnTile()
        {
            var emptyTiles = _board.GetEmptyTiles();
            if (emptyTiles.Count > 0)
            {
                var index = _random.Next(emptyTiles.Count);
                emptyTiles[index].Value = _random.Next(0, 10) < 9 ? 2 : 4;
                LastSpawnedTileIndices = emptyTiles[index].GetPosition();
            }
        }

        public bool MoveTiles(MoveDirection direction)
        {
            LastMergedTileIndices.Clear();
            bool wasChange = false;
            var tiles = _board.GetTiles();

            for (int i = 0; i < 4; i++)
            {
                List<Tile> line = GetLine(i, direction, tiles);

                bool reversed = (direction == MoveDirection.DOWN || direction == MoveDirection.RIGHT);
                
                if (reversed) line.Reverse();
                wasChange |= ProcessLine(line);
                if (reversed) line.Reverse();
            }

            if (wasChange)
            {
                SpawnTile();
                UpdateScore();
                CheckGameOver();
            }

            return wasChange;
        }

        private List<Tile> GetLine(int index, MoveDirection direction, Tile[,] tiles)
        {
            return direction switch
            {
                MoveDirection.LEFT  => Enumerable.Range(0, 4).Select(col => tiles[index, col]).ToList(),
                MoveDirection.RIGHT => Enumerable.Range(0, 4).Select(col => tiles[index, col]).ToList(),
                MoveDirection.UP    => Enumerable.Range(0, 4).Select(row => tiles[row, index]).ToList(),
                MoveDirection.DOWN  => Enumerable.Range(0, 4).Select(row => tiles[row, index]).ToList(),
                _ => throw new ArgumentException()
            };
        }

        private bool ProcessLine(IList<Tile> line) 
        {
            bool wasChange = false;
            // Shift non-zero tiles to the front
            int insertIndex = 0;
            for (int i = 0; i < line.Count; i++)
            {
                if (line[i].Value != 0)
                {
                    if (i != insertIndex)
                    {
                        line[insertIndex].Value = line[i].Value;
                        line[i].Value = 0;
                        wasChange = true;
                    }
                    insertIndex++;
                }
            }

            // Merge tiles
            for (int i = 0; i < line.Count - 1; i++)
            {
                if (line[i].Value != 0 && line[i].Value == line[i + 1].Value)
                {
                    line[i].Value *= 2;
                    line[i + 1].Value = 0;
                    line[i].Merged = true;
                    wasChange = true;
                    i++;
                }
            }

            // Shift again after merging
            insertIndex = 0;
            for (int i = 0; i < line.Count; i++)
            {
                if (line[i].Value != 0)
                {
                    if (i != insertIndex)
                    {
                        line[insertIndex].Value = line[i].Value;
                        line[insertIndex].Merged = line[i].Merged;
                        line[i].Value = 0;
                        line[i].Merged = false;
                        wasChange = true;
                    }
                    insertIndex++;
                }
            }

            // Remember merged tiles' positions and reset their merged status
            for (int i = 0; i < line.Count; i++)
            {
                if (line[i].Merged)
                {
                    LastMergedTileIndices.Add(line[i].GetPosition());
                    line[i].Merged = false;
                }
            }

            return wasChange;
        }

        private void UpdateScore()
        {
            int newScore = 0;
            var tiles = _board.GetTiles();
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (tiles[row, col].Value == 0) continue;

                    newScore += tiles[row, col].Value;
                    int timesMerged = (int)Math.Log(tiles[row, col].Value, 2) - 1;
                    newScore += (int)Math.Pow(3, timesMerged);
                }
            }
            _score = newScore;
        }

        private bool HasMoves()
        {
            for(int row = 0; row < 4; row++)
            {
                for(int col = 0; col < 4; col++)
                {
                    int value = _board.GetTile(row, col).Value;

                    // Empty tile found                    
                    if (value == 0) return true;

                    // Check right neighbor
                    if (col < 3 && _board.GetTile(row, col + 1).Value == value) return true;

                    //Check bottom neighbor
                    if (row < 3 && _board.GetTile(row + 1, col).Value == value) return true;
                }
            }

            return false;
        }

        private void CheckGameOver()
        {
            if(!HasMoves())
            {
                OnGameOver();
            }
        }

        private void OnGameOver()
        {
            GameOver?.Invoke();
        }
    }
}
