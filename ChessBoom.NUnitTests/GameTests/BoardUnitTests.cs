using NUnit.Framework;
using ChessBoom.GameBoard;

namespace ChessBoom.NUnitTests.GameTests
{
    public class BoardUnitTests
    {
        private Board _board = null!;

        [SetUp]
        public void Setup()
        {
            _board = new Board();
        }

        /// <summary>
        /// Test creating a board and printing out the pieces in the proper order
        /// </summary>
        [Test]
        public void BoardCreationTest()
        {
            //Setting up arbitrary placement of valid pieces
            _board.CreatePiece('p', 1, 1);
            _board.CreatePiece('p', 1, 2);
            _board.CreatePiece('p', 1, 3);
            _board.CreatePiece('p', 1, 4);
            _board.CreatePiece('p', 1, 5);
            _board.CreatePiece('k', 2, 4);
            _board.CreatePiece('q', 2, 5);
            _board.CreatePiece('r', 5, 1);
            _board.CreatePiece('R', 7, 7);


            Assert.AreEqual("pppppkqrR", _board.ToString());
        }

        /// <summary>
        /// Test creating an invalid board by placing pieces out of the range of the board both greater and less than row/column size.
        /// </summary>
        [Test]
        public void InvalidPositionThrowsBoardCreationTest()
        {
            //Out of bounds negative
            var exception1 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('p', -1, 1);
                });

            var exception2 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('p', 1, -1);
                });

            //Out of bounds positive (board is 8x8, we index by 0)
            var exception3 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('p', 8, 1);
                });

            var exception4 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('p', 1, 8);
                });

            //Do not need to do for all messages, validating here is enough
            if (exception1 != null)
                Assert.AreEqual("Coordinate (1, -1) is an invalid coordinate (x, y).", exception1.Message);
            if (exception2 != null)
                Assert.AreEqual("Coordinate (-1, 1) is an invalid coordinate (x, y).", exception2.Message);
        }
        /// <summary>
        /// If an invalid piece, such as 'h' is created, we get an ArgumentException
        /// </summary>
        [Test]
        public void InvalidPieceThrowsBoardCreationTest()
        {
            var exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('h', 1, 1);
                });

            if (exception != null)
                Assert.AreEqual(exception.Message, "Error. h is an invalid piece type.");
        }

        /// <summary>
        /// If we create a piece with an uppercase character, it is white, else it is black.
        /// </summary>
        [Test]
        public void PieceTeamProperlySet()
        {
            _board.CreatePiece('K', 0, 1);
            _board.CreatePiece('k', 0, 2);
            _board.CreatePiece('Q', 1, 1);
            _board.CreatePiece('q', 1, 2);
            _board.CreatePiece('B', 2, 1);
            _board.CreatePiece('b', 2, 2);
            _board.CreatePiece('N', 3, 1);
            _board.CreatePiece('n', 3, 2);
            _board.CreatePiece('R', 4, 1);
            _board.CreatePiece('r', 4, 2);
            _board.CreatePiece('P', 5, 1);
            _board.CreatePiece('p', 5, 2);


            var whiteKing = _board.GetPiece((0, 1));
            var blackKing = _board.GetPiece((0, 2));
            var whiteQueen = _board.GetPiece((1, 1));
            var blackQueen = _board.GetPiece((1, 2));
            var whiteBishop = _board.GetPiece((2, 1));
            var blackBishop = _board.GetPiece((2, 2));
            var whiteKnight = _board.GetPiece((3, 1));
            var blackKnight = _board.GetPiece((3, 2));
            var whiteRook = _board.GetPiece((4, 1));
            var blackRook = _board.GetPiece((4, 2));
            var whitePawn = _board.GetPiece((5, 1));
            var blackPawn = _board.GetPiece((5, 2));


            Assert.AreEqual(Player.White, whiteKing.GetPlayer());
            Assert.AreEqual(Player.Black, blackKing.GetPlayer());
            Assert.AreEqual(Player.White, whiteQueen.GetPlayer());
            Assert.AreEqual(Player.Black, blackQueen.GetPlayer());
            Assert.AreEqual(Player.White, whiteBishop.GetPlayer());
            Assert.AreEqual(Player.Black, blackBishop.GetPlayer());
            Assert.AreEqual(Player.White, whiteKnight.GetPlayer());
            Assert.AreEqual(Player.Black, blackKnight.GetPlayer());
            Assert.AreEqual(Player.White, whiteRook.GetPlayer());
            Assert.AreEqual(Player.Black, blackRook.GetPlayer());
            Assert.AreEqual(Player.White, whitePawn.GetPlayer());
            Assert.AreEqual(Player.Black, blackPawn.GetPlayer());
        }

        /// <summary>
        /// If there is an empty spot on the board and we try and get a piece from there, return null.
        /// </summary>
        [Test]
        public void BoardReturnsNullPieceOnEmptySpot()
        {
            var piece = _board.GetPiece((5, 5));

            Assert.AreEqual(null, piece);
        }
    }
}
