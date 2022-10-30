using NUnit.Framework;
using ChessBoom.GameBoard;

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
            Assert.AreEqual(square1, "c3");
            Assert.AreEqual(square2, "g1");
            Assert.AreEqual(square3, "h8");
            Assert.AreEqual(square4, "b6");
            Assert.AreEqual(square5, "d5");
            Assert.AreEqual(square6, "f3");
            Assert.AreEqual(square7, "b8");
            Assert.AreEqual(square8, "a1");
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
            Assert.AreEqual(coordinate1, (2, 2));
            Assert.AreEqual(coordinate2, (6, 0));
            Assert.AreEqual(coordinate3, (7, 7));
            Assert.AreEqual(coordinate4, (1, 5));
            Assert.AreEqual(coordinate5, (3, 4));
            Assert.AreEqual(coordinate6, (5, 2));
            Assert.AreEqual(coordinate7, (1, 7));
            Assert.AreEqual(coordinate8, (0, 0));
        }
    }
}