using NUnit.Framework;
using ChessBoom.GameBoard;

namespace ChessBoom.NUnitTests.GameTests
{
    public class PieceUnitTests
    {
        private Board _board = null!;
        private Player _whitePlayer;
        private Player _blackPlayer;

        [SetUp]
        public void Setup()
        {
            _board = new Board();
            _whitePlayer = Player.White;
            _blackPlayer = Player.Black;
        }
        /// <summary>
        /// Ensure that a Piece outputs a capital letter of on the White team and a lowercase letter if on the Black team.
        /// </summary>
        [Test]
        public void EnsurePieceOutputsRightCapitalizationPerTeam()
        {
            Piece whiteKing = new King(_board, _whitePlayer, 0, 0);
            Piece blackKing = new King(_board, _blackPlayer, 0, 1);
            Piece whiteQueen = new Queen(_board, _whitePlayer, 1, 0);
            Piece blackQueen = new Queen(_board, _blackPlayer, 1, 1);
            Piece whiteBishop = new Bishop(_board, _whitePlayer, 2, 0);
            Piece blackBishop = new Bishop(_board, _blackPlayer, 2, 1);
            Piece whiteKnight = new Knight(_board, _whitePlayer, 3, 0);
            Piece blackKnight = new Knight(_board, _blackPlayer, 3, 1);
            Piece whiteRook = new Rook(_board, _whitePlayer, 4, 0);
            Piece blackRook = new Rook(_board, _blackPlayer, 4, 1);
            Piece whitePawn = new Pawn(_board, _whitePlayer, 5, 0);
            Piece blackPawn = new Pawn(_board, _blackPlayer, 5, 1);


            Assert.AreEqual("K", whiteKing.ToString());
            Assert.AreEqual("k", blackKing.ToString());
            Assert.AreEqual("Q", whiteQueen.ToString());
            Assert.AreEqual("q", blackQueen.ToString());
            Assert.AreEqual("B", whiteBishop.ToString());
            Assert.AreEqual("b", blackBishop.ToString());
            Assert.AreEqual("N", whiteKnight.ToString());
            Assert.AreEqual("n", blackKnight.ToString());
            Assert.AreEqual("R", whiteRook.ToString());
            Assert.AreEqual("r", blackRook.ToString());
            Assert.AreEqual("P", whitePawn.ToString());
            Assert.AreEqual("p", blackPawn.ToString());
        }

        /// <summary>
        /// Check which squares the bishop can move to.
        /// </summary>
        [Test]
        public void BishopFreeMovementSquareTest()
        {
            _board.CreatePiece('B', 1, 1);
            var bishop = _board.GetPiece((1, 1));

            Assert.IsNotNull(bishop);
            // Free movement
            if (bishop != null)
            {
                var movementSquares = bishop.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 9);
                Assert.Contains((0, 0), movementSquares);
                Assert.Contains((0, 2), movementSquares);
                Assert.Contains((2, 0), movementSquares);
                Assert.Contains((2, 2), movementSquares);
                Assert.Contains((3, 3), movementSquares);
                Assert.Contains((4, 4), movementSquares);
                Assert.Contains((5, 5), movementSquares);
                Assert.Contains((6, 6), movementSquares);
                Assert.Contains((7, 7), movementSquares);
            }

            _board.CreatePiece('P', 6, 6);
            // Blocked by allied piece
            if (bishop != null)
            {
                var movementSquares = bishop.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 7);
                Assert.Contains((0, 0), movementSquares);
                Assert.Contains((0, 2), movementSquares);
                Assert.Contains((2, 0), movementSquares);
                Assert.Contains((2, 2), movementSquares);
                Assert.Contains((3, 3), movementSquares);
                Assert.Contains((4, 4), movementSquares);
                Assert.Contains((5, 5), movementSquares);
            }

            _board.CreatePiece('p', 3, 3);
            // Blocked by enemy piece
            if (bishop != null)
            {
                var movementSquares = bishop.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 5);
                Assert.Contains((0, 0), movementSquares);
                Assert.Contains((0, 2), movementSquares);
                Assert.Contains((2, 0), movementSquares);
                Assert.Contains((2, 2), movementSquares);
                Assert.Contains((3, 3), movementSquares);
            }
        }
    }
}
