using NUnit.Framework;
using ChessBoom.Views.GameBoard;

namespace ChessBoom.NUnitTests.GameTests
{
    public class GameHelpersUnitTests
    {
        /// <summary>
        /// Test the helper function for getting a square's name from its coordinates
        /// </summary>
        [Test]
        public void GetSquareFromCoordinateTest()
        {
            string square1 = GameHelpers.GetSquareFromCoordinate((2, 2));
            string square2 = GameHelpers.GetSquareFromCoordinate((6, 0));
            string square3 = GameHelpers.GetSquareFromCoordinate((7, 7));
            string square4 = GameHelpers.GetSquareFromCoordinate((1, 5));
            string square5 = GameHelpers.GetSquareFromCoordinate((3, 4));
            string square6 = GameHelpers.GetSquareFromCoordinate((5, 2));
            string square7 = GameHelpers.GetSquareFromCoordinate((1, 7));
            string square8 = GameHelpers.GetSquareFromCoordinate((0, 0));
            Assert.AreEqual("c3", square1);
            Assert.AreEqual("g1", square2);
            Assert.AreEqual("h8", square3);
            Assert.AreEqual("b6", square4);
            Assert.AreEqual("d5", square5);
            Assert.AreEqual("f3", square6);
            Assert.AreEqual("b8", square7);
            Assert.AreEqual("a1", square8);
        }

        /// <summary>
        /// Test the helper function for getting a square's name from illegal coordinates
        /// </summary>
        [Test]
        public void GetOoBSquareFromCoordinateTest()
        {
            var exception1 = Assert.Throws<ArgumentException>(
                delegate
                {
                    GameHelpers.GetSquareFromCoordinate((-1, 0));
                });
            var exception2 = Assert.Throws<ArgumentException>(
                delegate
                {
                    GameHelpers.GetSquareFromCoordinate((8, 15));
                });

            if (exception1 != null)
                Assert.AreEqual("Coordinate (-1, 0) is an invalid square", exception1.Message);
            if (exception2 != null)
                Assert.AreEqual("Coordinate (8, 15) is an invalid square", exception2.Message);
        }

        /// <summary>
        /// Test the helper function for getting the coordinates from a square's name
        /// </summary>
        [Test]
        public void GetCoordinateFromSquareTest()
        {
            (int, int) coordinate1 = GameHelpers.GetCoordinateFromSquare("c3");
            (int, int) coordinate2 = GameHelpers.GetCoordinateFromSquare("g1");
            (int, int) coordinate3 = GameHelpers.GetCoordinateFromSquare("h8");
            (int, int) coordinate4 = GameHelpers.GetCoordinateFromSquare("b6");
            (int, int) coordinate5 = GameHelpers.GetCoordinateFromSquare("d5");
            (int, int) coordinate6 = GameHelpers.GetCoordinateFromSquare("f3");
            (int, int) coordinate7 = GameHelpers.GetCoordinateFromSquare("b8");
            (int, int) coordinate8 = GameHelpers.GetCoordinateFromSquare("a1");
            Assert.AreEqual((2, 2), coordinate1);
            Assert.AreEqual((6, 0), coordinate2);
            Assert.AreEqual((7, 7), coordinate3);
            Assert.AreEqual((1, 5), coordinate4);
            Assert.AreEqual((3, 4), coordinate5);
            Assert.AreEqual((5, 2), coordinate6);
            Assert.AreEqual((1, 7), coordinate7);
            Assert.AreEqual((0, 0), coordinate8);
        }

        /// <summary>
        /// Test the helper function for getting a coordinate from an illegal square
        /// </summary>
        [Test]
        public void GetOoBCoordinateFromSquareTest()
        {
            var exception1 = Assert.Throws<ArgumentException>(
                delegate
                {
                    GameHelpers.GetCoordinateFromSquare("abc");
                });
            var exception2 = Assert.Throws<ArgumentException>(
                delegate
                {
                    GameHelpers.GetCoordinateFromSquare("11");
                });
            var exception3 = Assert.Throws<ArgumentException>(
                delegate
                {
                    GameHelpers.GetCoordinateFromSquare("bb");
                });

            if (exception1 != null)
                Assert.AreEqual("abc is not properly formatted.", exception1.Message);
            if (exception2 != null)
                Assert.AreEqual("11 does not have a proper column coordinate.", exception2.Message);
            if (exception3 != null)
                Assert.AreEqual("bb does not have a proper row coordinate.", exception3.Message);
        }

        /// <summary>
        /// Test the helper function for getting all legal squares from a movement vector and starting position
        /// </summary>
        [Test]
        public void GetMovementSquaresOoBTest()
        {
            List<(int, int)> movementSquares = new List<(int, int)>();
            Board board = new Board();
            Player player = Player.White;

            // Should return immediately, adding no movement squares, despite all hit squares being valid
            GameHelpers.GetVectorMovementSquares(ref movementSquares, board, player, (-1, 4), (1, 0));

            Assert.AreEqual(0, movementSquares.Count);
        }
    }
}