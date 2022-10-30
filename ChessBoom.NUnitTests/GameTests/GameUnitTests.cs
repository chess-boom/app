using NUnit.Framework;
using ChessBoom.GameBoard;

namespace ChessBoom.NUnitTests.GameTests
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
        /// Test creating a standard game and printing out the board state properly
        /// </summary>
        [Test]
        public void CheckDefaultFENSetupTest()
        {
            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual(fen, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        /// <summary>
        /// Make some moves and ensure the board is updated properly
        /// </summary>
        [Test]
        public void MoveTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("b1", "c3");

            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual(fen, "rnbqkbnr/pppppppp/8/8/4P3/2N5/PPPP1PPP/R1BQKBNR w KQkq - 2 2");
        }
    }
}