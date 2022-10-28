using NUnit.Framework;
using ChessBoom.GameBoard;

namespace ChessBoom.NUnitTests
{
    public class GameUnitTests
    {
        private Game _game = null!;

        [SetUp]
        public void Setup()
        {
            _game = new Game();
        }

        /// <summary>
        /// Test creating a game and printing out the board state properly
        /// </summary>
        [Test]
        public void CheckProperFENSetupTest()
        {
            string fen = Game.CreateFENFromBoard(_game.m_board);
            // FAILS: Can not access Resources folder!
            Assert.Equals(fen, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }
    }
}