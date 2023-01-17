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
        if (_gameHandler.m_game is null || _gameHandler.m_board is null)
        {
            Assert.Fail();
            return;
        }
        var gameState = _gameHandler.m_game.m_gameState;
        var fen = Game.CreateFENFromBoard(_gameHandler.m_board);

        Assert.AreEqual(GameState.VictoryWhite, gameState);
        Assert.AreEqual("r4rk1/ppp2pQp/n2p1n2/5N2/5Bb1/6PB/PP2PK1P/R1R5 b - - 0 15", fen);
    }
}