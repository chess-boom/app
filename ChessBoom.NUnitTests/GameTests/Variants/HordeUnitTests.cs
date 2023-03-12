using NUnit.Framework;
using ChessBoom.Models.Game;
using ChessBoom.Models.Game.Pieces;

namespace ChessBoom.NUnitTests.GameTests.Variants;

public class HordeUnitTests
{
    private Game _game = null!;

    [SetUp]
    public void Setup()
    {
        _game = new Game(Variant.Horde);
    }

    /// <summary>
    /// Test creating a standard game and printing out the board state properly
    /// </summary>
    [Test]
    public void CheckDefaultFENSetupTest()
    {
        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbnr/pppppppp/8/1PP2PP1/PPPPPPPP/PPPPPPPP/PPPPPPPP/PPPPPPPP w kq - 0 1", fen);
    }

    /// <summary>
    /// Ensure en passant works
    /// </summary>
    [Test]
    public void EnPassantTest()
    {
        _game.MakePGNMove("g6");    // 1
        _game.MakePGNMove("e5");

        // Ensure the FEN enables an "en passant" capture on square e6
        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbnr/pppp1ppp/6P1/1PP1pP2/PPPPPPPP/PPPPPPPP/PPPPPPPP/PPPPPPPP w kq e6 0 2", fen);

        _game.MakePGNMove("fxe6");  // 2

        // Ensure the capture takes place
        fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbnr/pppp1ppp/4P1P1/1PP5/PPPPPPPP/PPPPPPPP/PPPPPPPP/PPPPPPPP b kq - 0 2", fen);

        _game.MakePGNMove("fxe6");
        _game.MakePGNMove("f5");    // 3
        _game.MakePGNMove("exf5");
        _game.MakePGNMove("h5");    // 4
        _game.MakePGNMove("fxg4");
        _game.MakePGNMove("h4");    // 5
        _game.MakePGNMove("hxg6");
        _game.MakePGNMove("h6");    // 6
        _game.MakePGNMove("Rxh6");
        _game.MakePGNMove("h5");    // 7
        _game.MakePGNMove("Rxh5");
        _game.MakePGNMove("h3");    // 8
        _game.MakePGNMove("Rxh3");
        _game.MakePGNMove("f4");    // 9
        _game.MakePGNMove("Rxg3");
        _game.MakePGNMove("f5");    // 10
        _game.MakePGNMove("Rxe3");
        _game.MakePGNMove("f4");    // 11

        // Ensure the FEN enables an "en passant" capture on square f3
        fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbn1/pppp2p1/6p1/1PP2P2/PPPPPPp1/PPPPr3/PPPPP1P1/PPPPPPPP b q f3 0 11", fen);

        _game.MakePGNMove("g3");
        _game.MakePGNMove("h3");    // 12

        // Ensure the FEN does NOT enable an "en passant" capture on square h2
        fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbn1/pppp2p1/6p1/1PP2P2/PPPPPP2/PPPPr1pP/PPPPP1P1/PPPPPPP1 b q - 0 12", fen);
    }

    /// <summary>
    /// Ensure white losing all their pieces changes the game state
    /// </summary>
    [Test]
    public void WhiteLossTest()
    {
        _game.MakePGNMove("h5");    // 1
        _game.MakePGNMove("e6");
        _game.MakePGNMove("h6");    // 2
        _game.MakePGNMove("Qxg5");
        _game.MakePGNMove("h4");    // 3
        _game.MakePGNMove("Qxh6");
        _game.MakePGNMove("h5");    // 4
        _game.MakePGNMove("Qxh5");
        _game.MakePGNMove("h3");    // 5
        _game.MakePGNMove("Qxh3");
        _game.MakePGNMove("h2");    // 6
        _game.MakePGNMove("Qxh2");
        _game.MakePGNMove("g5");    // 7
        _game.MakePGNMove("Qxg1");
        _game.MakePGNMove("g6");    // 8
        _game.MakePGNMove("Qxg2");
        _game.MakePGNMove("f6");    // 9
        _game.MakePGNMove("Qxg3");
        _game.MakePGNMove("f5");    // 10
        _game.MakePGNMove("Qxg6");
        _game.MakePGNMove("e5");    // 11
        _game.MakePGNMove("Qxf6");
        _game.MakePGNMove("f4");    // 12
        _game.MakePGNMove("Qxf5");
        _game.MakePGNMove("f3");    // 13
        _game.MakePGNMove("Qxf4");
        _game.MakePGNMove("f2");    // 14
        _game.MakePGNMove("Qxf3");
        _game.MakePGNMove("e4");    // 15
        _game.MakePGNMove("Qxf2");
        _game.MakePGNMove("e3");    // 16
        _game.MakePGNMove("Qxe1");
        _game.MakePGNMove("d5");    // 17
        _game.MakePGNMove("Qxe3");
        _game.MakePGNMove("d6");    // 18
        _game.MakePGNMove("Qxe4");
        _game.MakePGNMove("d4");    // 19
        _game.MakePGNMove("Qxe5");
        _game.MakePGNMove("d5");    // 20
        _game.MakePGNMove("Qxd6");
        _game.MakePGNMove("d3");    // 21
        _game.MakePGNMove("Qxd5");
        _game.MakePGNMove("d4");    // 22
        _game.MakePGNMove("Qxd4");
        _game.MakePGNMove("d3");    // 23
        _game.MakePGNMove("Qxd3");
        _game.MakePGNMove("c6");    // 24
        _game.MakePGNMove("Qxc2");
        _game.MakePGNMove("c5");    // 25
        _game.MakePGNMove("Qxc1");
        _game.MakePGNMove("c4");    // 26
        _game.MakePGNMove("Qxc4");
        _game.MakePGNMove("b6");    // 27
        _game.MakePGNMove("Qxc5");
        _game.MakePGNMove("b5");    // 28
        _game.MakePGNMove("Qxc6");
        _game.MakePGNMove("b4");    // 29
        _game.MakePGNMove("Qxb5");
        _game.MakePGNMove("b3");    // 30
        _game.MakePGNMove("Qxb6");
        _game.MakePGNMove("b5");    // 31
        _game.MakePGNMove("Qxb5");
        _game.MakePGNMove("b4");    // 32
        _game.MakePGNMove("Qxb4");
        _game.MakePGNMove("b3");    // 33
        _game.MakePGNMove("Qxb3");
        _game.MakePGNMove("a5");    // 34
        _game.MakePGNMove("Qxa3");
        _game.MakePGNMove("a6");    // 35
        _game.MakePGNMove("Qxa6");
        _game.MakePGNMove("a3");    // 36
        _game.MakePGNMove("Qxa3");
        _game.MakePGNMove("a2");    // 37
        Assert.AreEqual(_game.m_gameState, GameState.InProgress);
        _game.MakePGNMove("Qxa2#");
        Assert.AreEqual(_game.m_gameState, GameState.VictoryBlack);

        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnb1kbnr/pppp1ppp/4p3/8/8/8/q7/8 w kq - 0 38", fen);
    }

    /// <summary>
    /// Ensure black checkmate changes the game state
    /// </summary>
    [Test]
    public void BlackCheckmateTest()
    {
        _game.MakePGNMove("e5");
        _game.MakePGNMove("a6");
        _game.MakePGNMove("g6");
        _game.MakePGNMove("b6");
        _game.MakePGNMove("e6");
        _game.MakePGNMove("c6");
        Assert.AreEqual(_game.m_gameState, GameState.InProgress);
        _game.MakePGNMove("gxf7#");
        Assert.AreEqual(_game.m_gameState, GameState.VictoryWhite);

        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbnr/3ppPpp/ppp1P3/1PP2P2/PPPP1PPP/PPPPPPPP/PPPPPPPP/PPPPPPPP b kq - 0 4", fen);
    }

    /// <summary>
    /// Ensure a draw is made in the case of stalemate
    /// </summary>
    [Test]
    public void StalemateTest()
    {
        _game.MakePGNMove("h5");    // 1
        _game.MakePGNMove("e6");
        _game.MakePGNMove("h6");    // 2
        _game.MakePGNMove("Qxg5");
        _game.MakePGNMove("h4");    // 3
        _game.MakePGNMove("Qxh6");
        _game.MakePGNMove("h5");    // 4
        _game.MakePGNMove("Qxh5");
        _game.MakePGNMove("h3");    // 5
        _game.MakePGNMove("Qxh3");
        _game.MakePGNMove("h2");    // 6
        _game.MakePGNMove("Qxh2");
        _game.MakePGNMove("g5");    // 7
        _game.MakePGNMove("Qxg1");
        _game.MakePGNMove("g6");    // 8
        _game.MakePGNMove("Qxg2");
        _game.MakePGNMove("f6");    // 9
        _game.MakePGNMove("Qxg3");
        _game.MakePGNMove("f5");    // 10
        _game.MakePGNMove("Qxg6");
        _game.MakePGNMove("e5");    // 11
        _game.MakePGNMove("Qxf6");
        _game.MakePGNMove("f4");    // 12
        _game.MakePGNMove("Qxf5");
        _game.MakePGNMove("f3");    // 13
        _game.MakePGNMove("Qxf4");
        _game.MakePGNMove("f2");    // 14
        _game.MakePGNMove("Qxf3");
        _game.MakePGNMove("e4");    // 15
        _game.MakePGNMove("Qxf2");
        _game.MakePGNMove("e3");    // 16
        _game.MakePGNMove("Qxe1");
        _game.MakePGNMove("d5");    // 17
        _game.MakePGNMove("Qxe3");
        _game.MakePGNMove("d6");    // 18
        _game.MakePGNMove("Qxe4");
        _game.MakePGNMove("d4");    // 19
        _game.MakePGNMove("Qxe5");
        _game.MakePGNMove("d5");    // 20
        _game.MakePGNMove("Qxd6");
        _game.MakePGNMove("d3");    // 21
        _game.MakePGNMove("Qxd5");
        _game.MakePGNMove("d4");    // 22
        _game.MakePGNMove("Qxd4");
        _game.MakePGNMove("d3");    // 23
        _game.MakePGNMove("Qxd3");
        _game.MakePGNMove("c6");    // 24
        _game.MakePGNMove("Qxc2");
        _game.MakePGNMove("c5");    // 25
        _game.MakePGNMove("Qxc1");
        _game.MakePGNMove("c4");    // 26
        _game.MakePGNMove("Qxc4");
        _game.MakePGNMove("b6");    // 27
        _game.MakePGNMove("Qxc5");
        _game.MakePGNMove("b5");    // 28
        _game.MakePGNMove("Qxc6");
        _game.MakePGNMove("b4");    // 29
        _game.MakePGNMove("Qxb5");
        _game.MakePGNMove("b3");    // 30
        _game.MakePGNMove("Qxb6");
        _game.MakePGNMove("b5");    // 31
        _game.MakePGNMove("Qxb5");
        _game.MakePGNMove("b4");    // 32
        _game.MakePGNMove("Qxb4");
        _game.MakePGNMove("b3");    // 33
        _game.MakePGNMove("Qxb3");
        _game.MakePGNMove("a5");    // 34
        _game.MakePGNMove("Qxa3");
        _game.MakePGNMove("a6");    // 35
        _game.MakePGNMove("Qxa2");
        _game.MakePGNMove("axb7");    // 36
        Assert.AreEqual(_game.m_gameState, GameState.InProgress);
        _game.MakePGNMove("Bxb7");
        Assert.AreEqual(_game.m_gameState, GameState.Draw);

        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rn2kbnr/pbpp1ppp/4p3/8/8/8/q7/P7 w kq - 0 37", fen);
    }
}