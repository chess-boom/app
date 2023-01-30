using System;
using System.Collections.Generic;
using ChessBoom.Models.Game.Pieces;

namespace ChessBoom.Models.Game.Rulesets;

public class Atomic : Ruleset
{
    public override void Capture(Piece attacker, Board board, string square)
    {
        var capturedPiece = board.GetPiece(GameHelpers.GetCoordinateFromSquare(square));
        capturedPiece?.Destroy();
        attacker.Destroy();

        // Get all surrounding pieces (not pawns) and destroy them as well
        (int col, int row) coordinate = GameHelpers.GetCoordinateFromSquare(square);
        List<Piece> surroundingPieces = new List<Piece>();
        for (int xIndex = coordinate.col - 1; xIndex <= coordinate.col + 1; xIndex++)
        {
            for (int yIndex = coordinate.row - 1; yIndex <= coordinate.row + 1; yIndex++)
            {
                Piece? piece = board.GetPiece((xIndex, yIndex));
                if (piece is not null)
                {
                    surroundingPieces.Add(piece);
                }
            }
        }

        foreach (Piece piece in surroundingPieces)
        {
            if (piece.GetType() == typeof(Pawn))
            {
                continue;
            }
            piece.Destroy();
        }
    }

    public override bool IsInCheck(Player player, Board board)
    {
        throw new NotImplementedException();
    }

    public override bool CanCastle(Board board, Player player, Castling side)
    {
        throw new NotImplementedException();
    }

    public override void Castle(Board board, Player player, Castling side)
    {
        throw new NotImplementedException();
    }

    public override string GetInitialRookSquare(Player player, Castling side)
    {
        throw new NotImplementedException();
    }

    public override bool IsIllegalBoardState(Board board)
    {
        throw new NotImplementedException();
    }

    public override void AssessBoardState(Game game, Board board)
    {
        throw new NotImplementedException();
    }
}