using NUnit.Framework;
using ChessBoom.Models.Game;

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
            Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", fen);
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
            Assert.AreEqual("r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3", fen);
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
            Assert.AreEqual("rn2kbnr/ppp1pppp/8/3b4/8/8/PPPP1PPP/R1BQKBNR w KQkq - 0 5", fen);
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
            Assert.AreEqual("rnbqkbnr/1pp1pppp/p7/3pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3", fen);

            // Ensure the capture takes place
            _game.MakeExplicitMove("e5", "d6");
            fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual("rnbqkbnr/1pp1pppp/p2P4/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3", fen);
        }

        /// <summary>
        /// Ensure promotion works
        /// </summary>
        [Test]
        public void PromotionTest()
        {
            // TODO: Update this test once requesting a piece type works! Currently (Nov. 3, 2022) always promotes to queen
            _game.MakeExplicitMove("h2", "h4");
            _game.MakeExplicitMove("b7", "b5");
            _game.MakeExplicitMove("h4", "h5");
            _game.MakeExplicitMove("b5", "b4");
            _game.MakeExplicitMove("h5", "h6");
            _game.MakeExplicitMove("b4", "b3");
            _game.MakeExplicitMove("h6", "g7");
            _game.MakeExplicitMove("b3", "a2");

            // Ensure the promotion takes place
            _game.MakeExplicitMove("g7", "h8");
            _game.MakeExplicitMove("a2", "b1");
            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual("rnbqkbnQ/p1pppp1p/8/8/8/8/1PPPPPP1/RqBQKBNR w KQq - 0 6", fen);
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
                Assert.AreEqual("Error. Piece R on a1 is unable to move to a3!", exception1.Message);
            if (exception2 != null)
                Assert.AreEqual("Error. Piece N on g1 is unable to move to e2!", exception2.Message);
            if (exception3 != null)
                Assert.AreEqual("Error. Piece P on e5 is unable to move to f6!", exception3.Message);
            if (exception4 != null)
                Assert.AreEqual("Piece on square h3 not found!", exception4.Message);
            if (exception5 != null)
                Assert.AreEqual($"i2 does not have a proper column coordinate.", exception5.Message);
            if (exception6 != null)
                Assert.AreEqual("Piece p can not move because it is not Black\'s turn!", exception6.Message);
        }

        /// <summary>
        /// Ensure castling kingside works
        /// </summary>
        [Test]
        public void KingsideCastlingTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("e7", "e5");
            _game.MakeExplicitMove("g1", "f3");
            _game.MakeExplicitMove("g8", "f6");
            _game.MakeExplicitMove("f1", "c4");
            _game.MakeExplicitMove("f8", "c5");
            _game.MakeExplicitMove("e1", "O-O");
            _game.MakeExplicitMove("e8", "O-O");

            // Ensure the FEN allows kingside castling
            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual("rnbq1rk1/pppp1ppp/5n2/2b1p3/2B1P3/5N2/PPPP1PPP/RNBQ1RK1 w - - 6 5", fen);
        }

        /// <summary>
        /// Ensure castling queenside works
        /// </summary>
        [Test]
        public void QueensideCastlingTest()
        {
            _game.MakeExplicitMove("d2", "d4");
            _game.MakeExplicitMove("d7", "d5");
            _game.MakeExplicitMove("d1", "d3");
            _game.MakeExplicitMove("d8", "d6");
            _game.MakeExplicitMove("b1", "c3");
            _game.MakeExplicitMove("b8", "c6");
            _game.MakeExplicitMove("c1", "e3");
            _game.MakeExplicitMove("c8", "e6");
            _game.MakeExplicitMove("e1", "O-O-O");
            _game.MakeExplicitMove("e8", "O-O-O");

            // Ensure the FEN allows queenside castling
            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual("2kr1bnr/ppp1pppp/2nqb3/3p4/3P4/2NQB3/PPP1PPPP/2KR1BNR w - - 8 6", fen);
        }

        /// <summary>
        /// Ensure that castling through check is not allowed
        /// </summary>
        [Test]
        public void CastlingThroughCheckTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("b7", "b6");
            _game.MakeExplicitMove("f1", "a6");
            _game.MakeExplicitMove("c8", "a6");
            _game.MakeExplicitMove("g1", "f3");
            _game.MakeExplicitMove("e7", "e6");

            var exception = Assert.Throws<GameplayErrorException>(
                delegate
                {
                    // Player attempts to castle through check
                    _game.MakeExplicitMove("e1", "O-O");
                });

            if (exception != null)
                Assert.AreEqual("Castling is illegal in this situation!", exception.Message);
        }

        /// <summary>
        /// Ensure that castling while in check is not allowed
        /// </summary>
        [Test]
        public void CastlingOutOfCheckTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("e7", "e5");
            _game.MakeExplicitMove("f1", "c4");
            _game.MakeExplicitMove("d7", "d5");
            _game.MakeExplicitMove("d2", "d4");
            _game.MakeExplicitMove("d5", "c4");
            _game.MakeExplicitMove("g1", "f3");
            _game.MakeExplicitMove("f8", "b4");

            var exception = Assert.Throws<GameplayErrorException>(
                delegate
                {
                    // Player attempts to castle out of check
                    _game.MakeExplicitMove("e1", "O-O");
                });

            if (exception != null)
                Assert.AreEqual("Castling is illegal in this situation!", exception.Message);
        }

        /// <summary>
        /// Ensure that castling after moving the king is not allowed
        /// </summary>
        [Test]
        public void CastlingAfterMovingTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("e7", "e5");
            _game.MakeExplicitMove("g1", "f3");
            _game.MakeExplicitMove("g8", "f6");
            _game.MakeExplicitMove("f1", "c4");
            _game.MakeExplicitMove("f8", "c5");
            _game.MakeExplicitMove("e1", "e2");
            _game.MakeExplicitMove("e8", "e7");
            _game.MakeExplicitMove("e2", "e1");
            _game.MakeExplicitMove("e7", "e8");

            var exception = Assert.Throws<GameplayErrorException>(
                delegate
                {
                    // Player attempts to castle out of check
                    _game.MakeExplicitMove("e1", "O-O");
                });

            if (exception != null)
                Assert.AreEqual("Castling is illegal in this situation!", exception.Message);
        }

        /// <summary>
        /// Ensure that checks may be blocked, captured, or moved out of
        /// </summary>
        [Test]
        public void HandlingCheckTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("f7", "f5");
            _game.MakeExplicitMove("d1", "h5");
            _game.MakeExplicitMove("g7", "g6"); // Check is blocked
            _game.MakeExplicitMove("h5", "g6");
            _game.MakeExplicitMove("h7", "g6"); // Check is captured
            _game.MakeExplicitMove("f1", "b5");
            _game.MakeExplicitMove("f8", "h6");
            _game.MakeExplicitMove("b5", "d7");
            _game.MakeExplicitMove("e8", "f7"); // Check is sidestepped

            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual("rnbq2nr/pppBpk2/6pb/5p2/4P3/8/PPPP1PPP/RNB1K1NR w KQ - 1 6", fen);
        }

        /// <summary>
        /// Ensure that making illegal moves while in check throws GameplayErrorExceptions
        /// </summary>
        [Test]
        public void IllegalCheckMoveTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("f7", "f5");
            _game.MakeExplicitMove("d1", "h5");

            var exception1 = Assert.Throws<GameplayErrorException>(
                delegate
                {
                    // Player attempts to walk towards the check
                    _game.MakeExplicitMove("e8", "f7");
                });

            var exception2 = Assert.Throws<GameplayErrorException>(
                delegate
                {
                    // Player attempts to play another random move
                    _game.MakeExplicitMove("g7", "g5");
                });

            if (exception1 != null)
                Assert.AreEqual("Error! Illegal move!", exception1.Message);
            if (exception2 != null)
                Assert.AreEqual("Error! Illegal move!", exception2.Message);
        }

        /// <summary>
        /// Ensure white checkmate changes the game state
        /// </summary>
        [Test]
        public void WhiteCheckmateTest()
        {
            _game.MakeExplicitMove("e2", "e4");
            _game.MakeExplicitMove("f7", "f6");
            _game.MakeExplicitMove("f1", "c4");
            _game.MakeExplicitMove("g7", "g5");
            Assert.AreEqual(_game.m_gameState, GameState.InProgress);
            _game.MakeExplicitMove("d1", "h5");
            Assert.AreEqual(_game.m_gameState, GameState.VictoryWhite);

            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual("rnbqkbnr/ppppp2p/5p2/6pQ/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 1 3", fen);
        }

        /// <summary>
        /// Ensure black checkmate changes the game state
        /// </summary>
        [Test]
        public void BlackCheckmateTest()
        {
            _game.MakeExplicitMove("f2", "f3");
            _game.MakeExplicitMove("e7", "e6");
            _game.MakeExplicitMove("g2", "g4");
            Assert.AreEqual(_game.m_gameState, GameState.InProgress);
            _game.MakeExplicitMove("d8", "h4");
            Assert.AreEqual(_game.m_gameState, GameState.VictoryBlack);

            string fen = Game.CreateFENFromBoard(_game.m_board);
            Assert.AreEqual("rnb1kbnr/pppp1ppp/4p3/8/6Pq/5P2/PPPPP2P/RNBQKBNR w KQkq - 1 3", fen);
        }

        /// <summary>
        /// Ensure moves can not be played in an improper game state
        /// </summary>
        [Test]
        public void MoveAfterGameEndTest()
        {
            _game.MakeExplicitMove("f2", "f3");
            _game.MakeExplicitMove("e7", "e6");
            _game.MakeExplicitMove("g2", "g4");
            Assert.AreEqual(GameState.InProgress, _game.m_gameState);
            _game.MakeExplicitMove("d8", "h4");
            Assert.AreEqual(GameState.VictoryBlack, _game.m_gameState);

            var exception = Assert.Throws<GameplayErrorException>(
                delegate
                {
                    // Player attempts to play another random move
                    _game.MakeExplicitMove("e2", "e3");
                });

            if (exception != null)
                Assert.AreEqual("Game is not in progress! Illegal move.", exception.Message);
        }
    }
}