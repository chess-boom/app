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
            Piece whiteKing = new King(_board, _whitePlayer, (0, 0));
            Piece blackKing = new King(_board, _blackPlayer, (0, 1));
            Piece whiteQueen = new Queen(_board, _whitePlayer, (1, 0));
            Piece blackQueen = new Queen(_board, _blackPlayer, (1, 1));
            Piece whiteBishop = new Bishop(_board, _whitePlayer, (2, 0));
            Piece blackBishop = new Bishop(_board, _blackPlayer, (2, 1));
            Piece whiteKnight = new Knight(_board, _whitePlayer, (3, 0));
            Piece blackKnight = new Knight(_board, _blackPlayer, (3, 1));
            Piece whiteRook = new Rook(_board, _whitePlayer, (4, 0));
            Piece blackRook = new Rook(_board, _blackPlayer, (4, 1));
            Piece whitePawn = new Pawn(_board, _whitePlayer, (5, 0));
            Piece blackPawn = new Pawn(_board, _blackPlayer, (5, 1));


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
            _board.CreatePiece('B', (1, 1));
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

            _board.CreatePiece('P', (6, 6));
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

            _board.CreatePiece('p', (3, 3));
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

        /// <summary>
        /// Check which squares the king can move to.
        /// </summary>
        [Test]
        public void KingFreeMovementSquareTest()
        {
            _board.CreatePiece('K', (0, 0));
            var king1 = _board.GetPiece((0, 0));

            Assert.IsNotNull(king1);
            // Free movement
            if (king1 != null)
            {
                var movementSquares = king1.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 3);
                Assert.Contains((1, 0), movementSquares);
                Assert.Contains((1, 1), movementSquares);
                Assert.Contains((0, 1), movementSquares);
            }

            _board.CreatePiece('K', (3, 3));
            var king2 = _board.GetPiece((3, 3));

            Assert.IsNotNull(king2);
            // Free movement
            if (king2 != null)
            {
                var movementSquares = king2.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 8);
                Assert.Contains((2, 2), movementSquares);
                Assert.Contains((2, 3), movementSquares);
                Assert.Contains((2, 4), movementSquares);
                Assert.Contains((3, 2), movementSquares);
                Assert.Contains((3, 4), movementSquares);
                Assert.Contains((4, 2), movementSquares);
                Assert.Contains((4, 3), movementSquares);
                Assert.Contains((4, 4), movementSquares);
            }

            _board.CreatePiece('P', (4, 4));
            // Blocked by allied piece
            if (king2 != null)
            {
                var movementSquares = king2.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 7);
                Assert.Contains((2, 2), movementSquares);
                Assert.Contains((2, 3), movementSquares);
                Assert.Contains((2, 4), movementSquares);
                Assert.Contains((3, 2), movementSquares);
                Assert.Contains((3, 4), movementSquares);
                Assert.Contains((4, 2), movementSquares);
                Assert.Contains((4, 3), movementSquares);
            }

            _board.CreatePiece('p', (3, 2));
            // Blocked by enemy piece
            if (king2 != null)
            {
                var movementSquares = king2.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 7);
                Assert.Contains((2, 2), movementSquares);
                Assert.Contains((2, 3), movementSquares);
                Assert.Contains((2, 4), movementSquares);
                Assert.Contains((3, 2), movementSquares);
                Assert.Contains((3, 4), movementSquares);
                Assert.Contains((4, 2), movementSquares);
                Assert.Contains((4, 3), movementSquares);
            }
        }

        /// <summary>
        /// Check which squares the knight can move to.
        /// </summary>
        [Test]
        public void KnightFreeMovementSquareTest()
        {
            _board.CreatePiece('N', (0, 0));
            var knight1 = _board.GetPiece((0, 0));

            Assert.IsNotNull(knight1);
            // Corner movement
            if (knight1 != null)
            {
                var movementSquares = knight1.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 2);
                Assert.Contains((2, 1), movementSquares);
                Assert.Contains((1, 2), movementSquares);
            }

            _board.CreatePiece('N', (4, 4));
            var knight2 = _board.GetPiece((4, 4));
            // Free movement
            if (knight2 != null)
            {
                var movementSquares = knight2.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 8);
                Assert.Contains((2, 3), movementSquares);
                Assert.Contains((2, 5), movementSquares);
                Assert.Contains((3, 6), movementSquares);
                Assert.Contains((3, 2), movementSquares);
                Assert.Contains((5, 6), movementSquares);
                Assert.Contains((5, 2), movementSquares);
                Assert.Contains((6, 5), movementSquares);
                Assert.Contains((6, 3), movementSquares);
            }

            _board.CreatePiece('P', (2, 3));
            // Blocked by allied piece
            if (knight2 != null)
            {
                var movementSquares = knight2.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 7);
                Assert.Contains((2, 5), movementSquares);
                Assert.Contains((3, 6), movementSquares);
                Assert.Contains((3, 2), movementSquares);
                Assert.Contains((5, 6), movementSquares);
                Assert.Contains((5, 2), movementSquares);
                Assert.Contains((6, 5), movementSquares);
                Assert.Contains((6, 3), movementSquares);
            }

            _board.CreatePiece('p', (2, 5));
            // Blocked by enemy piece
            if (knight2 != null)
            {
                var movementSquares = knight2.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 7);
                Assert.Contains((2, 5), movementSquares);
                Assert.Contains((3, 6), movementSquares);
                Assert.Contains((3, 2), movementSquares);
                Assert.Contains((5, 6), movementSquares);
                Assert.Contains((5, 2), movementSquares);
                Assert.Contains((6, 5), movementSquares);
                Assert.Contains((6, 3), movementSquares);
            }
        }

        /// <summary>
        /// Check which squares the pawn can move to.
        /// </summary>
        [Test]
        public void PawnFreeMovementSquareTest()
        {
            _board.CreatePiece('P', (0, 0));
            var pawn1 = _board.GetPiece((0, 0));

            Assert.IsNotNull(pawn1);
            // Double movement
            if (pawn1 != null)
            {
                var movementSquares = pawn1.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 2);
                Assert.Contains((0, 1), movementSquares);
                Assert.Contains((0, 2), movementSquares);
            }

            _board.CreatePiece('p', (4, 6));
            _board.CreatePiece('N', (3, 5));
            _board.CreatePiece('N', (4, 4));
            _board.CreatePiece('n', (5, 5));
            var pawn2 = _board.GetPiece((4, 6));
            // Blocked and capture movement
            if (pawn2 != null)
            {
                var movementSquares = pawn2.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 2);
                Assert.Contains((4, 5), movementSquares);
                Assert.Contains((3, 5), movementSquares);
            }

            _board.CreatePiece('P', (7, 1));
            _board.CreatePiece('p', (7, 2));
            _board.m_enPassant = (6, 2);
            var pawn3 = _board.GetPiece((7, 1));
            // Double movement blocked with en passant
            if (pawn3 != null)
            {
                var movementSquares = pawn3.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 1);
                Assert.Contains((6, 2), movementSquares);
            }
        }

        /// <summary>
        /// Check which squares the queen can move to.
        /// </summary>
        [Test]
        public void QueenFreeMovementSquareTest()
        {
            _board.CreatePiece('Q', (1, 1));
            var queen1 = _board.GetPiece((1, 1));

            Assert.IsNotNull(queen1);
            // Free movement
            if (queen1 != null)
            {
                var movementSquares = queen1.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 23);
                Assert.Contains((0, 0), movementSquares);
                Assert.Contains((0, 1), movementSquares);
                Assert.Contains((0, 2), movementSquares);
                Assert.Contains((1, 0), movementSquares);
                Assert.Contains((2, 0), movementSquares);

                // Vertical
                Assert.Contains((1, 2), movementSquares);
                Assert.Contains((1, 3), movementSquares);
                Assert.Contains((1, 4), movementSquares);
                Assert.Contains((1, 5), movementSquares);
                Assert.Contains((1, 6), movementSquares);
                Assert.Contains((1, 7), movementSquares);

                // Horizontal
                Assert.Contains((2, 1), movementSquares);
                Assert.Contains((3, 1), movementSquares);
                Assert.Contains((4, 1), movementSquares);
                Assert.Contains((5, 1), movementSquares);
                Assert.Contains((6, 1), movementSquares);
                Assert.Contains((7, 1), movementSquares);

                // Diagonal
                Assert.Contains((2, 2), movementSquares);
                Assert.Contains((3, 3), movementSquares);
                Assert.Contains((4, 4), movementSquares);
                Assert.Contains((5, 5), movementSquares);
                Assert.Contains((6, 6), movementSquares);
                Assert.Contains((7, 7), movementSquares);
            }

            _board.CreatePiece('P', (6, 6));
            _board.CreatePiece('P', (6, 1));
            _board.CreatePiece('P', (1, 6));
            // Blocked by allied piece
            if (queen1 != null)
            {
                var movementSquares = queen1.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 17);
                Assert.Contains((0, 0), movementSquares);
                Assert.Contains((0, 1), movementSquares);
                Assert.Contains((0, 2), movementSquares);
                Assert.Contains((1, 0), movementSquares);
                Assert.Contains((2, 0), movementSquares);

                // Vertical
                Assert.Contains((1, 2), movementSquares);
                Assert.Contains((1, 3), movementSquares);
                Assert.Contains((1, 4), movementSquares);
                Assert.Contains((1, 5), movementSquares);

                // Horizontal
                Assert.Contains((2, 1), movementSquares);
                Assert.Contains((3, 1), movementSquares);
                Assert.Contains((4, 1), movementSquares);
                Assert.Contains((5, 1), movementSquares);

                // Diagonal
                Assert.Contains((2, 2), movementSquares);
                Assert.Contains((3, 3), movementSquares);
                Assert.Contains((4, 4), movementSquares);
                Assert.Contains((5, 5), movementSquares);
            }

            _board.CreatePiece('p', (3, 3));
            _board.CreatePiece('p', (1, 3));
            _board.CreatePiece('p', (3, 1));
            // Blocked by enemy piece
            if (queen1 != null)
            {
                var movementSquares = queen1.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 11);
                Assert.Contains((0, 0), movementSquares);
                Assert.Contains((0, 1), movementSquares);
                Assert.Contains((0, 2), movementSquares);
                Assert.Contains((1, 0), movementSquares);
                Assert.Contains((2, 0), movementSquares);

                // Vertical
                Assert.Contains((1, 2), movementSquares);
                Assert.Contains((1, 3), movementSquares);

                // Horizontal
                Assert.Contains((2, 1), movementSquares);
                Assert.Contains((3, 1), movementSquares);

                // Diagonal
                Assert.Contains((2, 2), movementSquares);
                Assert.Contains((3, 3), movementSquares);
            }
        }

        /// <summary>
        /// Check which squares the rook can move to.
        /// </summary>
        [Test]
        public void RookFreeMovementSquareTest()
        {
            _board.CreatePiece('R', (1, 1));
            var rook = _board.GetPiece((1, 1));

            Assert.IsNotNull(rook);
            // Free movement
            if (rook != null)
            {
                var movementSquares = rook.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 14);
                Assert.Contains((0, 1), movementSquares);
                Assert.Contains((1, 0), movementSquares);

                // Vertical
                Assert.Contains((1, 2), movementSquares);
                Assert.Contains((1, 3), movementSquares);
                Assert.Contains((1, 4), movementSquares);
                Assert.Contains((1, 5), movementSquares);
                Assert.Contains((1, 6), movementSquares);
                Assert.Contains((1, 7), movementSquares);

                // Horizontal
                Assert.Contains((2, 1), movementSquares);
                Assert.Contains((3, 1), movementSquares);
                Assert.Contains((4, 1), movementSquares);
                Assert.Contains((5, 1), movementSquares);
                Assert.Contains((6, 1), movementSquares);
                Assert.Contains((7, 1), movementSquares);
            }

            _board.CreatePiece('P', (6, 1));
            _board.CreatePiece('P', (1, 6));
            // Blocked by allied piece
            if (rook != null)
            {
                var movementSquares = rook.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 10);
                Assert.Contains((0, 1), movementSquares);
                Assert.Contains((1, 0), movementSquares);

                // Vertical
                Assert.Contains((1, 2), movementSquares);
                Assert.Contains((1, 3), movementSquares);
                Assert.Contains((1, 4), movementSquares);
                Assert.Contains((1, 5), movementSquares);

                // Horizontal
                Assert.Contains((2, 1), movementSquares);
                Assert.Contains((3, 1), movementSquares);
                Assert.Contains((4, 1), movementSquares);
                Assert.Contains((5, 1), movementSquares);
            }

            _board.CreatePiece('p', (3, 3));
            _board.CreatePiece('p', (1, 3));
            _board.CreatePiece('p', (3, 1));
            // Blocked by enemy piece
            if (rook != null)
            {
                var movementSquares = rook.GetMovementSquares();
                Assert.AreEqual(movementSquares.Count, 6);
                Assert.Contains((0, 1), movementSquares);
                Assert.Contains((1, 0), movementSquares);

                // Vertical
                Assert.Contains((1, 2), movementSquares);
                Assert.Contains((1, 3), movementSquares);

                // Horizontal
                Assert.Contains((2, 1), movementSquares);
                Assert.Contains((3, 1), movementSquares);
            }
        }
    }
}
