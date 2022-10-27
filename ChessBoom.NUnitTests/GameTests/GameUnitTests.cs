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

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}