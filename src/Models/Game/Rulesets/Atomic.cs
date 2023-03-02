using System;
using System.Collections.Generic;
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
    /// TODO
    /// </summary>
    private List<Piece> GetSurroundingPieces(Board board, string square, bool includeOriginPiece = false, bool includePawns = false)
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
    /// TODO
    /// </summary>
    private bool CaptureExplodesKing(Player player, Board board, string square)
    {
        // Get all surrounding pieces (not pawns) and destroy them as well

        // TODO: Reimplement using GetSurroundingPieces
        (int col, int row) coordinate = GameHelpers.GetCoordinateFromSquare(square);
        List<Piece> explodingPieces = new List<Piece>();
        Piece? capturedPiece = board.GetPiece(GameHelpers.GetCoordinateFromSquare(square));
        if (capturedPiece is not null) explodingPieces.Add(capturedPiece);
        for (int xIndex = coordinate.col - 1; xIndex <= coordinate.col + 1; xIndex++)
        {
            for (int yIndex = coordinate.row - 1; yIndex <= coordinate.row + 1; yIndex++)
            {
                Piece? piece = board.GetPiece((xIndex, yIndex));
                if (piece is not null)
                {
                    explodingPieces.Add(piece);
                }
            }
        }

        foreach (Piece piece in explodingPieces)
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
    /// TODO
    /// </summary>
    /// <param name="player">The player whose king may be blown up</param>
    private bool CanExplodeKing(Player player, Board board)
    {
        // TODO
        // Get all allied pieces surrounding the king
        var king = GetKing(board, player);
        List<Piece> surroundingPieces = GetSurroundingPieces(board, GameHelpers.GetSquareFromCoordinate(king.GetCoordinates()), true, true);
        foreach (Piece piece in surroundingPieces)
        {
            if (piece.GetPlayer() != player)
            {
                surroundingPieces.Remove(piece);
            }
        }

        // Query if those squares are visible to enemy pieces
        foreach (Piece piece in surroundingPieces)
        {
            if (GameHelpers.IsSquareVisible(board, GameHelpers.GetOpponent(player), piece.GetCoordinates()))
            {
                return true;
            }
        }
        return false;
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
        // You may explode an opponents king, even while in check
        bool isInCheck = IsInCheck(GameHelpers.GetOpponent(board.m_playerToPlay), board);
        if (!isInCheck)
        {
            return isInCheck;
        }

        bool canExplodeKing = CanExplodeKing(board.m_playerToPlay, board);
        // TODO: Double-check this logic
        return (isInCheck && canExplodeKing);
    }

    public override void AssessBoardState(Game game, Board board)
    {
        // TODO

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
        throw new NotImplementedException();
    }

    public override Piece GetKing(Board board, Player player)
    {
        return Standard.Instance.GetKing(board, player);
    }
}