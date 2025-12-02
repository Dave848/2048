using NUnit.Framework;
using _2048.Models.GameLogic;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace _2048.Tests
{
    [TestFixture]
    public class GameEngineTests
    {
        private GameEngine _engine;

        [SetUp]
        public void Setup()
        {
            _engine = new GameEngine();
        }

        [Test]
        public void NewGame_InitialTilesEmptyScoreZero()
        {
            int[] tiles = _engine.GetTiles();
            Assert.That(tiles.All(t => t == 0), Is.True);
            Assert.That(_engine.GetScore(), Is.EqualTo(0));
        }

        [Test]
        public void SetGameState_ShouldSetBoardAndScore()
        {
            int[] initialBoard = new int[16];
            initialBoard[0] = 2;
            _engine.SetGameState(initialBoard, 42);

            int[] tiles = _engine.GetTiles();
            Assert.That(tiles[0], Is.EqualTo(2));
            Assert.That(_engine.GetScore(), Is.EqualTo(42));
        }

        [Test]
        public void SpawnInitialTiles_ShouldSpawnTwoTiles()
        {
            _engine.SpawnInitialTiles();
            int[] tiles = _engine.GetTiles();
            int nonZeroCount = tiles.Count(t => t != 0);

            Assert.That(nonZeroCount, Is.EqualTo(2));
            Assert.That(_engine.LastSpawnedTileIndices.Item1 >= 0, Is.True);
        }

        [Test]
        public void MoveTiles_Left_ShiftsAndMergesTilesCorrectly()
        {
            int[] board = new int[16]
            {
                2, 2, 0, 0,
                4, 0, 4, 0,
                2, 0, 0, 2,
                0, 0, 0, 0
            };
            _engine.SetGameState(board, 0);

            bool changed = _engine.MoveTiles(MoveDirection.LEFT);

            int[] expected = new int[16]
            {
                4, 0, 0, 0,
                8, 0, 0, 0,
                4, 0, 0, 0,
                0, 0, 0, 0
            };

            var tiles = _engine.GetTiles();
            for (int i = 0; i < 16; i++)
            {
                if (i == _engine.LastSpawnedTileIndices.Item1 * 4 + _engine.LastSpawnedTileIndices.Item2)
                    continue;
                
                Assert.That(tiles[i], Is.EqualTo(expected[i]));
            }

            Assert.That(changed, Is.True);
            Assert.That(_engine.LastMergedTileIndices.Count > 0, Is.True);
        }

        [Test]
        public void MoveTiles_NoChange_ReturnsFalse()
        {
            int[] board = new int[16]
            {
                2, 4, 8, 16,
                32, 64, 128, 256,
                512, 1024, 2, 4,
                8, 16, 32, 64
            };
            _engine.SetGameState(board, 0);

            bool changed = _engine.MoveTiles(MoveDirection.LEFT);

            Assert.That(changed, Is.False);
        }

        [Test]
        public void GameOver_ShouldTriggerEvent()
        {
            bool gameOverTriggered = false;
            _engine.GameOver += () => gameOverTriggered = true;

            int[] board = new int[16]
            {
                2,  4,  2,  4,
                4,  2,  4,  2,
                8,  4,  2,  4,
                16, 8, 16, 16
            };
            _engine.SetGameState(board, 0);

            _engine.MoveTiles(MoveDirection.LEFT);

            Assert.That(gameOverTriggered, Is.True);
        }

        [Test]
        public void Score_ShouldUpdateAfterMerge()
        {
            int[] board = new int[16]
            {
                2, 2, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0
            };
            _engine.SetGameState(board, 0);
            _engine.MoveTiles(MoveDirection.LEFT);

            int score = _engine.GetScore();
            Assert.That(score > 0, Is.True);
        }
    }
}