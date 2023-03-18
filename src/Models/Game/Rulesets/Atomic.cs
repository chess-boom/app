using System;
using System.Collections.Generic;
using System.Linq;
using ChessBoom.Models.Game.Pieces;

namespace ChessBoom.Models.Game.Rulesets;

public class Atomic : Ruleset
{
    private static readonly Atomic _instance = new();

    private Atomic()
    {
    }

    public static Atomic Instance => _instance;

    public override void Capture(Piece attacker, Board board, string square)
    {
        var capturedPiece = board.GetPiece(GameHelpers.GetCoordinateFromSquare(square));
        capturedPiece?.Destroy();
        attacker.Destroy();

        // Get all surrounding pieces (not pawns) and destroy them as well
        foreach (Piece piece in GetSurroundingPieces(board, square, true, false))
        {
            piece.Destroy();
        }
    }

    /// <summary>
    /// Retrieves all the pieces surrounding a specified square
    /// </summary>
    /// <param name="board">The board on which the other pieces exist</param>
    /// <param name="square">The square that is examined</param>
    /// <param name="includeOriginPiece">Flag for whether the piece on the specified square should be returned</param>
    /// <param name="includePawns">Flag for whether pawns should be returned</param>
    /// <returns>The list of pieces surrounding the square</returns>
    private static List<Piece> GetSurroundingPieces(Board board, string square, bool includeOriginPiece = false, bool includePawns = false)
    {
        List<Piece> surroundingPieces = new List<Piece>();

        (int col, int row) coordinate = GameHelpers.GetCoordinateFromSquare(square);
        for (int xIndex = coordinate.col - 1; xIndex <= coordinate.col + 1; xIndex++)
        {
            for (int yIndex = coordinate.row - 1; yIndex <= coordinate.row + 1; yIndex++)
            {
                if (!includeOriginPiece && xIndex == coordinate.col && yIndex == coordinate.row)
                {
                    continue;
                }

                Piece? piece = board.GetPiece((xIndex, yIndex));
                if (piece is not null)
                {
                    if (!includePawns && piece.GetType() == typeof(Pawn))
                    {
                        continue;
                    }
                    surroundingPieces.Add(piece);
                }
            }
        }

        return surroundingPieces;
    }

    /// <summary>
    /// Determines if a capture occurring on a specified square explodes a specific player's king
    /// </summary>
    /// <param name="player">The player whose king might explode</param>
    /// <param name="board">The board on which the pieces exist</param>
    /// <param name="square">The square that is examined</param>
    /// <returns>Whether or not the player's king explodes from a capture</returns>
    private bool CaptureExplodesKing(Player player, Board board, string square)
    {
        foreach (Piece piece in GetSurroundingPieces(board, square, true, false))
        {
            if (piece.GetType() == typeof(King)
                || piece.GetPlayer() == player)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determines if a player's king can be blown up
    /// </summary>
    /// <param name="player">The player whose king might explode</param>
    /// <param name="board">The board on which the pieces exist</param>
    /// <returns>Whether or not an opponent can blow up the player's king</returns>
    private bool CanExplodeKing(Player player, Board board)
    {
        // Get all allied pieces surrounding the king
        var king = GetKingOrNull(board, player);
        if (king is null)
        {
            return false;
        }
        List<Piece> surroundingPieces = GetSurroundingPieces(board, GameHelpers.GetSquareFromCoordinate(king.GetCoordinates()), true, true);
        surroundingPieces = surroundingPieces.Where(piece => piece.GetPlayer() == player).ToList();

        // Query if those squares are visible to enemy pieces
        return surroundingPieces.Any(piece => GameHelpers.IsSquareVisible(board, GameHelpers.GetOpponent(player), piece.GetCoordinates()));
    }

    public override bool IsInCheck(Player player, Board board)
    {
        return Standard.Instance.IsInCheck(player, board);
    }

    public override bool CanCastle(Board board, Player player, Castling side)
    {
        return Standard.Instance.CanCastle(board, player, side);
    }

    public override void Castle(Board board, Player player, Castling side)
    {
        Standard.Instance.Castle(board, player, side);
    }

    public override string GetInitialRookSquare(Player player, Castling side)
    {
        return Standard.Instance.GetInitialRookSquare(player, side);
    }

    public override bool IsIllegalBoardState(Board board)
    {
        if (AreKingsTouching(board))
        {
            return false;
        }

        // Check for exploded kings
        if (GetKingOrNull(board, GameHelpers.GetOpponent(board.m_playerToPlay)) is null)
        {
            return true;
        }

        return Standard.Instance.IsIllegalBoardState(board);
    }

    public override void AssessBoardState(Game game, Board board)
    {
        // Check for exploded kings
        var whiteKing = GetKingOrNull(board, Player.White);
        var blackKing = GetKingOrNull(board, Player.Black);
        if (whiteKing is null || blackKing is null)
        {
            if (whiteKing is null && blackKing is null)
            {
                Console.WriteLine("Error! Neither king could be found!");
                game.m_gameState = GameState.Aborted;
                return;
            }

            if (whiteKing is null)
            {
                game.m_gameState = GameState.VictoryBlack;
                return;
            }
            else
            {
                game.m_gameState = GameState.VictoryWhite;
                return;
            }
        }

        // 50-move rule
        if (board.m_halfmoveClock >= (2 * k_progressMoveLimit))
        {
            game.m_gameState = GameState.Draw;
            return;
        }

        // Threefold repetition
        if (game.HasThreefoldRepetition())
        {
            game.m_gameState = GameState.Draw;
            return;
        }

        var pieces = GameHelpers.GetPlayerPieces(board.m_playerToPlay, board);
        var legalMoveExists = false;

        foreach (var piece in pieces)
        {
            var pieceMoves = piece.GetMovementSquares();

            foreach (var move in pieceMoves)
            {
                var testGame = new Game();
                var testBoard = Game.CreateBoardFromFEN(testGame, Game.CreateFENFromBoard(board));

                try
                {
                    var testPiece = testBoard.GetPiece(piece.GetCoordinates());
                    if (testPiece is not null)
                    {
                        testPiece.MovePiece(move);
                    }
                    else
                    {
                        Console.WriteLine("Error! Test piece from duplicated board not found!");
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine($"Error! {e}");
                }
                catch (GameplayErrorException e)
                {
                    Console.WriteLine($"Error! {e}");
                }

                testBoard.m_playerToPlay = GameHelpers.GetOpponent(testBoard.m_playerToPlay);
                if (!IsIllegalBoardState(testBoard))
                {
                    legalMoveExists = true;
                    break;
                }
            }

            if (legalMoveExists)
            {
                break;
            }
        }

        if (legalMoveExists) return;
        // If next player is in checkmate or has lost their king, game over.
        if (IsInCheck(board.m_playerToPlay, board) || GetKingOrNull(board, board.m_playerToPlay) is null)
        {
            game.m_gameState = (board.m_playerToPlay == Player.Black) ? GameState.VictoryWhite : GameState.VictoryBlack;
        }
        else
        {
            game.m_gameState = GameState.Draw;
        }
    }

    public override Piece GetKing(Board board, Player player)
    {
        return Standard.Instance.GetKing(board, player);
    }

    /// <summary>
    /// Retrieves a player's king. Null if it does not exist
    /// </summary>
    /// <param name="board">The board on which the king exists</param>
    /// <param name="player">The player whose king might explode</param>
    /// <returns>The player's king. Null if not found</returns>
    private Piece? GetKingOrNull(Board board, Player player)
    {
        try
        {
            return GetKing(board, player);
        }
        catch (GameplayErrorException)
        {
            return null;
        }
    }

    /// <summary>
    /// Checks if the black and white kings are adjacent
    /// </summary>
    /// <param name="board">The board on which the kings exist</param>
    /// <returns>True if the players' kings are adjacent. False otherwise</returns>
    private bool AreKingsTouching(Board board)
    {
        var whiteKing = GetKingOrNull(board, Player.White);
        var blackKing = GetKingOrNull(board, Player.Black);
        if (whiteKing is null || blackKing is null)
        {
            return false;
        }

        (int, int) whiteCoordinates = whiteKing.GetCoordinates();
        (int, int) blackCoordinates = blackKing.GetCoordinates();
        return (Math.Abs(whiteCoordinates.Item1 - blackCoordinates.Item1) <= 1 &&
            Math.Abs(whiteCoordinates.Item2 - blackCoordinates.Item2) <= 1);
    }
}