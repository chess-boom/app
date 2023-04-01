using System;
using System.Collections.Generic;

namespace ChessBoom.Models.Game.Pieces;

public class Rook : Piece
{
    /// <summary>
    /// Flag to keep track of if the king has moved
    /// </summary>
    private bool m_hasMoved;

    public Rook(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
    {
        m_hasMoved = false;
    }

    public override void Destroy()
    {
        try
        {
            var kingsideRookSquare = m_board.GetRuleset().GetInitialRookSquare(m_owner, Castling.Kingside);
            if (m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(kingsideRookSquare)) == this)
            {
                m_board.RemoveCastling(m_owner, Castling.Kingside);
            }

            var queensideRookSquare = m_board.GetRuleset().GetInitialRookSquare(m_owner, Castling.Queenside);
            if (m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(queensideRookSquare)) == this)
            {
                m_board.RemoveCastling(m_owner, Castling.Queenside);
            }
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Error! Castling rights removal failed.");
            return;
        }

        base.Destroy();
    }

    public override List<(int, int)> GetMovementSquares()
    {
        var movementSquares = new List<(int, int)>();
        var position = GetCoordinates();

        // Top
        var movementVector = (0, 1);
        GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
        // Left
        movementVector = (-1, 0);
        GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
        // Right
        movementVector = (1, 0);
        GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
        // Bottom
        movementVector = (0, -1);
        GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);

        return movementSquares;
    }

    public override void MovePiece((int, int) coordinate, Board.RequestPromotionPieceDelegate? requestPromotionPiece = null)
    {
        if (m_hasMoved)
        {
            base.MovePiece(coordinate, requestPromotionPiece);
            return;
        }
        if (m_board.m_game is null)
        {
            return;
        }

        try
        {
            var kingsideRookSquare = m_board.GetRuleset().GetInitialRookSquare(m_owner, Castling.Kingside);
            if (m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(kingsideRookSquare)) == this)
            {
                m_board.RemoveCastling(m_owner, Castling.Kingside);
            }

            var queensideRookSquare = m_board.GetRuleset().GetInitialRookSquare(m_owner, Castling.Queenside);
            if (m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(queensideRookSquare)) == this)
            {
                m_board.RemoveCastling(m_owner, Castling.Queenside);
            }
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Error! Castling rights removal failed.");
            return;
        }

        m_hasMoved = true;
        base.MovePiece(coordinate, requestPromotionPiece);
    }

    public override string ToString()
    {
        return (m_owner == Player.White) ? "R" : "r";
    }
}