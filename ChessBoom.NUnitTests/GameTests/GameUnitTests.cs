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
            _game.MakeExplicitMove("e7", "e5");
            _game.MakeExplicitMove("g1", "f3");
            _game.MakeExplicitMove("b8", "c6");
            _game.MakeExplicitMove("f1", "b5");

            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual(fen, "r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3");
        }

        /// <summary>
        /// Make some moves and captures
        /// </summary>
        [Test]
        public void CaptureTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("d7", "d5");
            _game.MakeExplicitMove("e4", "d5");
            _game.MakeExplicitMove("d8", "d5");
            _game.MakeExplicitMove("b1", "c3");
            _game.MakeExplicitMove("c8", "e6");
            _game.MakeExplicitMove("c3", "d5");
            _game.MakeExplicitMove("e6", "d5");

            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual(fen, "rn2kbnr/ppp1pppp/8/3b4/8/8/PPPP1PPP/R1BQKBNR w KQkq - 0 5");
        }

        /// <summary>
        /// Ensure en passant works
        /// </summary>
        [Test]
        public void EnPassantTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("a7", "a6");
            _game.MakeExplicitMove("e4", "e5");
            _game.MakeExplicitMove("d7", "d5");

            // Ensure the FEN enables an "en passant" capture on square d6
            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual(fen, "rnbqkbnr/1pp1pppp/p7/3pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3");

            // Ensure the capture takes place
            _game.MakeExplicitMove("e5", "d6");
            fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual(fen, "rnbqkbnr/1pp1pppp/p2P4/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3");
        }

        /// <summary>
        /// Attempt to make illegal moves
        /// </summary>
        [Test]
        public void IllegalMoveTest()
        {
            var exception1 = Assert.Throws<ArgumentException>(
                delegate
                {
                    // Rook attempting to jump over allied pawn
                    _game.MakeExplicitMove("a1", "a3");
                });
            var exception2 = Assert.Throws<ArgumentException>(
                delegate
                {
                    // Knight attempting to capture allied pawn
                    _game.MakeExplicitMove("g1", "e2");
                });

            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("a7", "a6");
            _game.MakeExplicitMove("e4", "e5");
            _game.MakeExplicitMove("f7", "f5");
            _game.MakeExplicitMove("a2", "a3");
            _game.MakeExplicitMove("a6", "a5");

            var exception3 = Assert.Throws<ArgumentException>(
                delegate
                {
                    // Pawn attempting to capture en passant, but too late
                    _game.MakeExplicitMove("e5", "f6");
                });

            var exception4 = Assert.Throws<ArgumentException>(
                delegate
                {
                    // Empty square attempting to move
                    _game.MakeExplicitMove("h3", "h4");
                });

            var exception5 = Assert.Throws<ArgumentException>(
                delegate
                {
                    // Knight attempting to move out of bounds
                    _game.MakeExplicitMove("g1", "i2");
                });

            var exception6 = Assert.Throws<GameplayErrorException>(
                delegate
                {
                    // Player attempts to move when it's not their turn
                    _game.MakeExplicitMove("e7", "e6");
                });

            if (exception1 != null)
                Assert.AreEqual(exception1.Message, "Error. Piece R on a1 is unable to move to a3!");
            if (exception2 != null)
                Assert.AreEqual(exception2.Message, "Error. Piece N on g1 is unable to move to e2!");
            if (exception3 != null)
                Assert.AreEqual(exception3.Message, "Error. Piece P on e5 is unable to move to f6!");
            if (exception4 != null)
                Assert.AreEqual(exception4.Message, "Piece on square h3 not found!");
            if (exception5 != null)
                Assert.AreEqual(exception5.Message, $"i2 does not have a proper column coordinate.");
            if (exception6 != null)
                Assert.AreEqual(exception6.Message, "Piece p can not move because it is not Black\'s turn!");
        }
    }
}