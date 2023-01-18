using NUnit.Framework;
using ChessBoom.Models.Game;

namespace ChessBoom.NUnitTests.GameTests;

public class GameHandlerUnitTests
{
    private GameHandler _gameHandler = null!;

    [SetUp]
    public void Setup()
    {
        App.SetWorkingDirectory();
        _gameHandler = new GameHandler();
    }

    /// <summary>
    /// Test the ability to properly load a game from a .PGN file
    /// </summary>
    [Test]
    public void LoadGameTest()
    {
        _gameHandler.LoadGame("Resources/sample.pgn");
        var gameState = _gameHandler.GetCurrentGameState();
        var fen = _gameHandler.GetCurrentFENPosition();

        Assert.AreEqual(GameState.VictoryWhite, gameState);
        Assert.AreEqual("r4rk1/ppp2pQp/n2p1n2/5N2/5Bb1/6PB/PP2PK1P/R1R5 b - - 0 15", fen);
    }
}