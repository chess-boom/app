using NUnit.Framework;
using ChessBoom.GameBoard;

namespace ChessBoom.NUnitTests.GameTests
{
    internal class BoardUnitTests
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
                Assert.AreEqual(exception1.Message, "Coordinate (1, -1) is an invalid coordinate (x, y).");
            if (exception2 != null)
            Assert.AreEqual(exception2.Message, "Coordinate (-1, 1) is an invalid coordinate (x, y).");
          
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
            _board.CreatePiece('R', 1, 1);
            _board.CreatePiece('k', 2, 2);

            var white = _board.GetPiece((1, 1));
            var black = _board.GetPiece((2, 2));

            Assert.AreEqual(white.GetPlayer(), Player.White);
            Assert.AreEqual(black.GetPlayer(), Player.Black);
        }


    }
}
