using NUnit.Framework;
using _2048.Models.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2048.Tests
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void Constructor_Default_InitializesEmptyBoard()
        {
            var board = new Board();
            var tiles = board.GetTiles();
            foreach (var tile in tiles)
            {
                Assert.That(tile.Value, Is.EqualTo(0));
            }
        }

        [Test]
        public void Constructor_WithInitialBoard_SetsTilesCorrectly()
        {
            int[] initialBoard = Enumerable.Range(1, 16).ToArray();
            var board = new Board(initialBoard);

            var tiles = board.GetTiles();
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int expected = initialBoard[row * 4 + col];
                    Assert.That(tiles[row, col].Value, Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void Constructor_WithWrongLength_ThrowsException()
        {
            int[] wrongBoard = new int[15];
            Assert.Throws<ArgumentException>(() => new Board(wrongBoard));
        }

        [Test]
        public void GetTile_ValidIndices_ReturnsTile()
        {
            var board = new Board();
            var tile = board.GetTile(2, 3);

            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.GetPosition(), Is.EqualTo((2, 3)));
        }

        [Test]
        public void GetTile_InvalidIndices_ThrowsException()
        {
            var board = new Board();
            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetTile(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetTile(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetTile(4, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetTile(0, 4));
        }

        [Test]
        public void GetEmptyTiles_ReturnsOnlyEmptyTiles()
        {
            var board = new Board();
            board.GetTile(0, 0).Value = 2;

            var emptyTiles = board.GetEmptyTiles();
            Assert.That(emptyTiles.All(t => t.Value == 0));
            Assert.That(emptyTiles.Any(t => t.GetPosition() == (0, 0)), Is.False);
        }

        [Test]
        public void SetRow_ChangesTilesAndReturnsTrue()
        {
            var board = new Board();
            var newRow = new List<Tile>
            {
                new Tile(2,0,0),
                new Tile(2,0,1),
                new Tile(0,0,2),
                new Tile(4,0,3)
            };

            bool changed = board.SetRow(0, newRow);
            Assert.That(changed, Is.True);

            for (int col = 0; col < 4; col++)
            {
                Assert.That(board.GetTile(0, col).Value, Is.EqualTo(newRow[col].Value));
            }
        }

        [Test]
        public void SetRow_NoChange_ReturnsFalse()
        {
            var board = new Board();
            var row = new List<Tile>
            {
                new Tile(0,0,0),
                new Tile(0,0,1),
                new Tile(0,0,2),
                new Tile(0,0,3)
            };
            bool changed = board.SetRow(0, row);
            Assert.That(changed, Is.False);
        }

        [Test]
        public void SetColumn_ChangesTilesAndReturnsTrue()
        {
            var board = new Board();
            var newCol = new List<Tile>
            {
                new Tile(2,0,0),
                new Tile(2,1,0),
                new Tile(0,2,0),
                new Tile(4,3,0)
            };

            bool changed = board.SetColumn(0, newCol);
            Assert.That(changed, Is.True);

            for (int row = 0; row < 4; row++)
            {
                Assert.That(board.GetTile(row, 0).Value, Is.EqualTo(newCol[row].Value));
            }
        }

        [Test]
        public void SetColumn_NoChange_ReturnsFalse()
        {
            var board = new Board();
            var col = new List<Tile>
            {
                new Tile(0,0,0),
                new Tile(0,1,0),
                new Tile(0,2,0),
                new Tile(0,3,0)
            };

            bool changed = board.SetColumn(0, col);
            Assert.That(changed, Is.False);
        }
    }
}