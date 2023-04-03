using NUnit.Framework;
using ChessBoom.Models.Game;
using ChessBoom.Models.Game.Pieces;

namespace ChessBoom.NUnitTests.GameTests.Variants;

public class AtomicUnitTests
{
    private Game _game = null!;

    [SetUp]
    public void Setup()
    {
        _game = new Game(Variant.Atomic);
    }

    /// <summary>
    /// Test creating a standard game and printing out the board state properly
    /// </summary>
    [Test]
    public void CheckDefaultFENSetupTest()
    {
        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", fen);
    }

    /// <summary>
    /// Make some moves and ensure the board is updated properly
    /// </summary>
    [Test]
    public void MoveTest()
    {
        _game.MakeExplicitMove("e2", "e4");
        _game.MakeExplicitMove("e7", "e5");
        _game.MakeExplicitMove("g1", "f3");
        _game.MakeExplicitMove("b8", "c6");
        _game.MakeExplicitMove("f1", "b5");

        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3", fen);
    }

    /// <summary>
    /// Make some moves and captures
    /// </summary>
    [Test]
    public void CaptureTest()
    {
        _game.MakeExplicitMove("e2", "e4");
        _game.MakeExplicitMove("d7", "d5");
        _game.MakeExplicitMove("e4", "d5");

        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbnr/ppp1pppp/8/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2", fen);
    }

    /// <summary>
    /// Ensure that an exploded king ends the game
    /// </summary>
    [Test]
    public void KingExplosionTest()
    {
        _game.MakeExplicitMove("e2", "e4");
        _game.MakeExplicitMove("d7", "d5");
        _game.MakeExplicitMove("e4", "d5");
        Assert.AreEqual(_game.m_gameState, GameState.InProgress);
        _game.MakeExplicitMove("d8", "d2");
        Assert.AreEqual(_game.m_gameState, GameState.VictoryBlack);

        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnb1kbnr/ppp1pppp/8/8/8/8/PPP2PPP/RN3BNR w KQkq - 0 3", fen);
    }


    /// <summary>
    /// Ensure en passant works
    /// </summary>
    [Test]
    public void EnPassantTest()
    {
        _game.MakeExplicitMove("e2", "e4");
        _game.MakeExplicitMove("a7", "a6");
        _game.MakeExplicitMove("e4", "e5");
        _game.MakeExplicitMove("d7", "d5");

        // Ensure the FEN enables an "en passant" capture on square d6
        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbnr/1pp1pppp/p7/3pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3", fen);

        // Ensure the capture takes place
        _game.MakeExplicitMove("e5", "d6");
        fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbqkbnr/1pp1pppp/p7/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3", fen);
    }

    /// <summary>
    /// Attempt to make illegal moves
    /// </summary>
    [Test]
    public void IllegalMoveTest()
    {
        var exception1 = Assert.Throws<ArgumentException>(
            delegate
            {
                // Rook attempting to jump over allied pawn
                _game.MakeExplicitMove("a1", "a3");
            });
        var exception2 = Assert.Throws<ArgumentException>(
            delegate
            {
                // Knight attempting to capture allied pawn
                _game.MakeExplicitMove("g1", "e2");
            });

        _game.MakeExplicitMove("e2", "e4");
        _game.MakeExplicitMove("a7", "a6");
        _game.MakeExplicitMove("e4", "e5");
        _game.MakeExplicitMove("f7", "f5");
        _game.MakeExplicitMove("a2", "a3");
        _game.MakeExplicitMove("a6", "a5");

        var exception4 = Assert.Throws<ArgumentException>(
            delegate
            {
                // Empty square attempting to move
                _game.MakeExplicitMove("h3", "h4");
            });

        var exception5 = Assert.Throws<ArgumentException>(
            delegate
            {
                // Knight attempting to move out of bounds
                _game.MakeExplicitMove("g1", "i2");
            });

        var exception6 = Assert.Throws<GameplayErrorException>(
            delegate
            {
                // Player attempts to move when it's not their turn
                _game.MakeExplicitMove("e7", "e6");
            });

        if (exception1 is not null)
            Assert.AreEqual("Error. Piece R on a1 is unable to move to a3!", exception1.Message);
        if (exception2 is not null)
            Assert.AreEqual("Error. Piece N on g1 is unable to move to e2!", exception2.Message);
        if (exception4 is not null)
            Assert.AreEqual("Piece on square h3 not found!", exception4.Message);
        if (exception5 is not null)
            Assert.AreEqual($"i2 does not have a proper column coordinate.", exception5.Message);
        if (exception6 is not null)
            Assert.AreEqual("Piece p can not move because it is not Black\'s turn!", exception6.Message);
    }

    /// <summary>
    /// Ensure that castling through check is not allowed
    /// </summary>
    [Test]
    public void CastlingThroughCheckTest()
    {
        _game.MakeExplicitMove("e2", "e4");
        _game.MakeExplicitMove("b7", "b6");
        _game.MakeExplicitMove("f1", "c4");
        _game.MakeExplicitMove("c8", "a6");
        _game.MakeExplicitMove("g1", "f3");
        _game.MakeExplicitMove("e7", "e6");
        _game.MakeExplicitMove("c4", "b3");
        _game.MakeExplicitMove("d7", "d6");

        var exception = Assert.Throws<GameplayErrorException>(
            delegate
            {
                // Player attempts to castle through check
                _game.MakeExplicitMove("e1", "O-O");
            });

        if (exception is not null)
            Assert.AreEqual("Castling is illegal in this situation!", exception.Message);
    }

    /// <summary>
    /// Ensure that castling while in check is not allowed
    /// </summary>
    [Test]
    public void CastlingOutOfCheckTest()
    {
        _game.MakeExplicitMove("e2", "e4");
        _game.MakeExplicitMove("e7", "e5");
        _game.MakeExplicitMove("f1", "c4");
        _game.MakeExplicitMove("d7", "d5");
        _game.MakeExplicitMove("d2", "d4");
        _game.MakeExplicitMove("d5", "c4");
        _game.MakeExplicitMove("g1", "f3");
        _game.MakeExplicitMove("f8", "b4");

        var exception = Assert.Throws<GameplayErrorException>(
            delegate
            {
                // Player attempts to castle out of check
                _game.MakeExplicitMove("e1", "O-O");
            });

        if (exception is not null)
            Assert.AreEqual("Castling is illegal in this situation!", exception.Message);
    }

    /// <summary>
    /// Ensure that checks may be blocked, captured, or moved out of
    /// </summary>
    [Test]
    public void HandlingCheckTest()
    {
        _game.MakePGNMove("e4");
        _game.MakePGNMove("f5");
        _game.MakePGNMove("Qh5+");
        _game.MakePGNMove("g6");    // Check is blocked
        _game.MakePGNMove("Qh6");
        _game.MakePGNMove("g5");
        _game.MakePGNMove("Qg6+");
        _game.MakePGNMove("hxg6");  // Check is captured
        _game.MakePGNMove("Bc4");
        _game.MakePGNMove("d6");
        _game.MakePGNMove("Bf7+");
        _game.MakePGNMove("Kd7");   // Check is sidestepped

        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnbq1bnr/pppkpB2/3p4/5pp1/4P3/8/PPPP1PPP/RNB1K1NR w KQ - 2 7", fen);
    }

    /// <summary>
    /// Ensure that making illegal moves while in check throws GameplayErrorExceptions
    /// </summary>
    [Test]
    public void IllegalCheckMoveTest()
    {
        _game.MakeExplicitMove("e2", "e4");
        _game.MakeExplicitMove("f7", "f5");
        _game.MakeExplicitMove("d1", "h5");

        var exception1 = Assert.Throws<GameplayErrorException>(
            delegate
            {
                // Player attempts to walk towards the check
                _game.MakeExplicitMove("e8", "f7");
            });

        var exception2 = Assert.Throws<GameplayErrorException>(
            delegate
            {
                // Player attempts to play another random move
                _game.MakeExplicitMove("g7", "g5");
            });

        if (exception1 is not null)
            Assert.AreEqual("Error! Illegal move!", exception1.Message);
        if (exception2 is not null)
            Assert.AreEqual("Error! Illegal move!", exception2.Message);
    }

    /// <summary>
    /// Ensure white checkmate changes the game state
    /// </summary>
    [Test]
    public void WhiteCheckmateTest()
    {
        _game.MakePGNMove("e4");
        _game.MakePGNMove("d5");
        _game.MakePGNMove("Qe2");
        _game.MakePGNMove("dxe4");
        Assert.AreEqual(_game.m_gameState, GameState.InProgress);
        _game.MakePGNMove("Qxe7#");
        Assert.AreEqual(_game.m_gameState, GameState.VictoryWhite);

        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnb3nr/ppp2ppp/8/8/8/8/PPPP1PPP/RNB1KBNR b KQkq - 0 3", fen);
    }

    /// <summary>
    /// Ensure black checkmate changes the game state
    /// </summary>
    [Test]
    public void BlackCheckmateTest()
    {
        _game.MakeExplicitMove("f2", "f3");
        _game.MakeExplicitMove("e7", "e6");
        _game.MakeExplicitMove("g2", "g4");
        Assert.AreEqual(_game.m_gameState, GameState.InProgress);
        _game.MakeExplicitMove("d8", "h4");
        Assert.AreEqual(_game.m_gameState, GameState.VictoryBlack);

        var fen = Game.CreateFENFromBoard(_game.m_board);
        Assert.AreEqual("rnb1kbnr/pppp1ppp/4p3/8/6Pq/5P2/PPPPP2P/RNBQKBNR w KQkq - 1 3", fen);
    }

    /// <summary>
    /// Ensure moves can not be played in an improper game state
    /// </summary>
    [Test]
    public void MoveAfterGameEndTest()
    {
        _game.MakeExplicitMove("f2", "f3");
        _game.MakeExplicitMove("e7", "e6");
        _game.MakeExplicitMove("g2", "g4");
        Assert.AreEqual(GameState.InProgress, _game.m_gameState);
        _game.MakeExplicitMove("d8", "h4");
        Assert.AreEqual(GameState.VictoryBlack, _game.m_gameState);

        var exception = Assert.Throws<GameplayErrorException>(
            delegate
            {
                // Player attempts to play another random move
                _game.MakeExplicitMove("e2", "e3");
            });

        if (exception is not null)
            Assert.AreEqual("Game is not in progress! Illegal move.", exception.Message);
    }

    /// <summary>
    /// Ensure a draw is made in the case of threefold repetition
    /// </summary>
    [Test]
    public void ThreefoldRepetitionTest()
    {
        _game.MakeExplicitMove("e2", "e4");
        _game.MakeExplicitMove("e7", "e5");
        _game.MakeExplicitMove("e1", "e2");
        _game.MakeExplicitMove("e8", "e7");
        _game.MakeExplicitMove("e2", "e1");
        _game.MakeExplicitMove("e7", "e8");
        _game.MakeExplicitMove("e1", "e2");
        _game.MakeExplicitMove("e8", "e7");
        _game.MakeExplicitMove("e2", "e1");
        _game.MakeExplicitMove("e7", "e8");
        _game.MakeExplicitMove("e1", "e2");
        Assert.AreEqual(GameState.InProgress, _game.m_gameState);
        _game.MakeExplicitMove("e8", "e7");
        // Threefold repetition occurs
        Assert.AreEqual(GameState.Draw, _game.m_gameState);

        var exception = Assert.Throws<GameplayErrorException>(
            delegate
            {
                // Player attempts to play another random move
                _game.MakeExplicitMove("e2", "e3");
            });

        if (exception is not null)
            Assert.AreEqual("Game is not in progress! Illegal move.", exception.Message);
    }

    /// <summary>
    /// Ensure a draw is made in the case of stalemate
    /// </summary>
    [Test]
    public void StalemateTest()
    {
        _game.MakePGNMove("e4");
        _game.MakePGNMove("d5");
        _game.MakePGNMove("Qg4");
        _game.MakePGNMove("a5");
        _game.MakePGNMove("Qxg7");
        _game.MakePGNMove("c5");
        _game.MakePGNMove("Ba6");
        _game.MakePGNMove("Qc7");
        _game.MakePGNMove("Bxb7");
        _game.MakePGNMove("dxe4");
        _game.MakePGNMove("a4");
        _game.MakePGNMove("Kf8");
        _game.MakePGNMove("Ra3");
        _game.MakePGNMove("Kg8");
        _game.MakePGNMove("Rg3+");
        _game.MakePGNMove("Kh8");
        _game.MakePGNMove("h4");
        _game.MakePGNMove("f5");
        _game.MakePGNMove("h5");
        _game.MakePGNMove("e5");
        _game.MakePGNMove("h6");
        _game.MakePGNMove("e4");
        _game.MakePGNMove("c4");
        _game.MakePGNMove("e3");
        _game.MakePGNMove("dxe3");
        _game.MakePGNMove("f4");
        _game.MakePGNMove("Rg7");
        _game.MakePGNMove("f3");
        Assert.AreEqual(GameState.InProgress, _game.m_gameState);
        _game.MakePGNMove("g3");
        // Stalemate occurs
        Assert.AreEqual(GameState.Draw, _game.m_gameState);

        var exception = Assert.Throws<GameplayErrorException>(
            delegate
            {
                // Player attempts to play another random move
                _game.MakePGNMove("Bc2");
            });

        if (exception is not null)
            Assert.AreEqual("Game is not in progress! Illegal move.", exception.Message);
    }

    /// <summary>
    /// Ensure a draw is made in the case of the fifty move rule
    /// </summary>
    [Test]
    public void FiftyMoveRuleTest()
    {
        var whitePawn = _game.m_board.GetPiece(GameHelpers.GetCoordinateFromSquare("e2"));
        var whiteQueen = _game.m_board.GetPiece(GameHelpers.GetCoordinateFromSquare("d1"));
        var blackPawn = _game.m_board.GetPiece(GameHelpers.GetCoordinateFromSquare("e7"));
        var blackQueen = _game.m_board.GetPiece(GameHelpers.GetCoordinateFromSquare("d8"));
        var blackKnight = _game.m_board.GetPiece(GameHelpers.GetCoordinateFromSquare("g8"));

        if (whitePawn is null
            || whiteQueen is null
            || blackPawn is null
            || blackQueen is null
            || blackKnight is null)
        {
            Assert.Fail();
            return;
        }

        var moveList = new List<(Piece, string)>()
        {
            (whitePawn, "e4"),
            (blackPawn, "e5"), // 1
            (whiteQueen, "h5"),
            (blackQueen, "h4"), // 2
            (whiteQueen, "h6"),
            (blackKnight, "e7"), // 3
            (whiteQueen, "g6"),
            (blackKnight, "g8"), // 4
            (whiteQueen, "f6"),
            (blackKnight, "e7"), // 5
            (whiteQueen, "d6"),
            (blackKnight, "g8"), // 6
            (whiteQueen, "c6"),
            (blackKnight, "e7"), // 7
            (whiteQueen, "b6"),
            (blackKnight, "g8"), // 8
            (whiteQueen, "a6"),
            (blackKnight, "e7"), // 9
            (whiteQueen, "a5"),
            (blackKnight, "g8"), // 10
            (whiteQueen, "b5"),
            (blackKnight, "e7"), // 11
            (whiteQueen, "c5"),
            (blackKnight, "g8"), // 12
            (whiteQueen, "d5"),
            (blackKnight, "e7"), // 13
            (whiteQueen, "d4"),
            (blackKnight, "g8"), // 14
            (whiteQueen, "c4"),
            (blackKnight, "e7"), // 15
            (whiteQueen, "b4"),
            (blackKnight, "g8"), // 16
            (whiteQueen, "a4"),
            (blackKnight, "e7"), // 17
            (whiteQueen, "a3"),
            (blackKnight, "g8"), // 18
            (whiteQueen, "b3"),
            (blackKnight, "e7"), // 19
            (whiteQueen, "c3"),
            (blackKnight, "g8"), // 20
            (whiteQueen, "d3"),
            (blackKnight, "e7"), // 21
            (whiteQueen, "e3"),
            (blackKnight, "g8"), // 22
            (whiteQueen, "f3"),
            (blackKnight, "e7"), // 23
            (whiteQueen, "g3"),
            (blackKnight, "g8"), // 24
            (whiteQueen, "h3"),
            (blackKnight, "e7"), // 25
            (whiteQueen, "g4"),
            (blackKnight, "g8"), // 26
            (whiteQueen, "f4"),
            (blackKnight, "e7"), // 27
            (whiteQueen, "f5"),
            (blackKnight, "g8"), // 28
            (whiteQueen, "g5"),
            (blackQueen, "h5"), // 29
            (whiteQueen, "h6"),
            (blackKnight, "e7"), // 30
            (whiteQueen, "g6"),
            (blackKnight, "g8"), // 31
            (whiteQueen, "f6"),
            (blackKnight, "e7"), // 32
            (whiteQueen, "d6"),
            (blackKnight, "g8"), // 33
            (whiteQueen, "c6"),
            (blackKnight, "e7"), // 34
            (whiteQueen, "b6"),
            (blackKnight, "g8"), // 35
            (whiteQueen, "a6"),
            (blackKnight, "e7"), // 36
            (whiteQueen, "a5"),
            (blackKnight, "g8"), // 37
            (whiteQueen, "b5"),
            (blackKnight, "e7"), // 38
            (whiteQueen, "c5"),
            (blackKnight, "g8"), // 39
            (whiteQueen, "d5"),
            (blackKnight, "e7"), // 40
            (whiteQueen, "d4"),
            (blackKnight, "g8"), // 41
            (whiteQueen, "c4"),
            (blackKnight, "e7"), // 42
            (whiteQueen, "b4"),
            (blackKnight, "g8"), // 43
            (whiteQueen, "a4"),
            (blackKnight, "e7"), // 44
            (whiteQueen, "a3"),
            (blackKnight, "g8"), // 45
            (whiteQueen, "b3"),
            (blackKnight, "e7"), // 46
            (whiteQueen, "c3"),
            (blackKnight, "g8"), // 47
            (whiteQueen, "d3"),
            (blackKnight, "e7"), // 48
            (whiteQueen, "e3"),
            (blackKnight, "g8"), // 49
            (whiteQueen, "f3"),
            (blackKnight, "e7"), // 50
            (whiteQueen, "g3"),
            (blackKnight, "g8") // 51
        };

        // A 'for' loop is preferred over 'foreach' since last move should be avoided
        for (var index = 0; index < moveList.Count - 1; index++)
        {
            _game.MakeMove(moveList[index].Item1, moveList[index].Item2);
        }

        Assert.AreEqual(GameState.InProgress, _game.m_gameState);
        _game.MakeMove(moveList[moveList.Count - 1].Item1, moveList[moveList.Count - 1].Item2);
        // Fifty move rule is enforced
        Assert.AreEqual(GameState.Draw, _game.m_gameState);

        var exception = Assert.Throws<GameplayErrorException>(
            delegate
            {
                // Player attempts to play another random move
                _game.MakeExplicitMove("e1", "e2");
            });

        if (exception is not null)
            Assert.AreEqual("Game is not in progress! Illegal move.", exception.Message);
    }

    /// <summary>
    /// Ensure the legal moves of a king can be retrieved properly
    /// </summary>
    [Test]
    public void KingGetLegalMovesTest()
    {
        Piece? king = _game.m_board.GetPiece(GameHelpers.GetCoordinateFromSquare("e1"));
        if (king is null || king.GetType() != typeof(King))
        {
            Assert.Fail("Failed test - king can not be found");
        }

        // Starting position: trapped
        if (king is not null)
        {
            var movementSquares = king.GetLegalMoves();
            Assert.AreEqual(0, movementSquares.Count);
        }

        _game.MakeExplicitMove("e2", "e4"); // e4
        _game.MakeExplicitMove("g7", "g6"); // g6
        _game.MakeExplicitMove("g1", "f3"); // Nf3
        _game.MakeExplicitMove("f8", "h6"); // Bh6
        _game.MakeExplicitMove("f1", "c4"); // Bc4
        _game.MakeExplicitMove("g8", "f6"); // Nf6
        _game.MakeExplicitMove("d2", "d3"); // d3
        _game.MakeExplicitMove("c7", "c6"); // c6
        // Can move or castle, but not into check (on d2)
        if (king is not null)
        {
            var movementSquares = king.GetLegalMoves();
            Assert.AreEqual(3, movementSquares.Count);
            Assert.Contains("e2", movementSquares);
            Assert.Contains("f1", movementSquares);
            Assert.Contains("O-O", movementSquares);
        }

        _game.MakeExplicitMove("d3", "d4"); // d4
        _game.MakeExplicitMove("h6", "d2"); // Bd2+
        // Can move out of check, but not castle or capture
        if (king is not null)
        {
            var movementSquares = king.GetLegalMoves();
            Assert.AreEqual(2, movementSquares.Count);
            Assert.Contains("e2", movementSquares);
            Assert.Contains("f1", movementSquares);
        }

        _game.MakePGNMove("Ke2");
        _game.MakePGNMove("b6");
        _game.MakePGNMove("Bd5");
        _game.MakePGNMove("d6");
        _game.MakePGNMove("Ng5");
        _game.MakePGNMove("Ba6+");

        // Can move out of check, but not into check, while in check
        if (king is not null)
        {
            var movementSquares = king.GetLegalMoves();
            Assert.AreEqual(1, movementSquares.Count);
            Assert.Contains("f3", movementSquares);
        }

        _game.MakePGNMove("Kf3");
        _game.MakePGNMove("Kd7");
        _game.MakePGNMove("Nxf7");
        _game.MakePGNMove("cxd5");
        _game.MakePGNMove("Kg4");
        _game.MakePGNMove("Ke6");

        // Can move adjacent to an opponent's king
        if (king is not null)
        {
            var movementSquares = king.GetLegalMoves();
            Assert.AreEqual(5, movementSquares.Count);
            Assert.Contains("f3", movementSquares);
            Assert.Contains("g3", movementSquares);
            Assert.Contains("h3", movementSquares);
            Assert.Contains("h4", movementSquares);
            Assert.Contains("f5", movementSquares);
        }

        _game.MakePGNMove("Kf5");
        _game.MakePGNMove("Kf6");

        // Can stay adjacent to an opponent's king
        if (king is not null)
        {
            var movementSquares = king.GetLegalMoves();
            Assert.AreEqual(4, movementSquares.Count);
            Assert.Contains("e5", movementSquares);
            Assert.Contains("e6", movementSquares);
            Assert.Contains("g4", movementSquares);
            Assert.Contains("g5", movementSquares);
        }
    }
}