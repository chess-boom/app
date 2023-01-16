using NUnit.Framework;
using ChessBoom.Models.Game;

namespace ChessBoom.NUnitTests.GameTests;

public class GameHandlerUnitTests
{
    private GameHandler _gameHandler = null!;

    [SetUp]
    public void Setup()
    {
        _gameHandler = new GameHandler();
    }

    /// <summary>
    /// Test
    /// </summary>
    [Test]
    public void Test()
    {
        App.SetWorkingDirectory();
        _gameHandler.LoadGame("Resources/sample.pgn");
        var square1 = GameHelpers.GetSquareFromCoordinate((2, 2));
        Assert.AreEqual("a1", "a1");
    }
}