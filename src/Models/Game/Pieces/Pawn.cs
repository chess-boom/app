using System;
using System.Collections.Generic;

namespace ChessBoom.Models.Game.Pieces;

public class Pawn : Piece
{
    public Pawn(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
    {
    }

    public override List<(int, int)> GetMovementSquares()
    {
        var movementSquares = new List<(int, int)>();

        var coordinates = GetCoordinates();
        (int, int) standardMove;
        (int, int) doubleMove;
        (int, int) captureLeft;
        (int, int) captureRight;

        if (m_owner == Player.White)
        {
            standardMove = GameHelpers.AddVector(coordinates, (0, 1));
            doubleMove = GameHelpers.AddVector(coordinates, (0, 2));
            captureLeft = GameHelpers.AddVector(coordinates, (-1, 1));
            captureRight = GameHelpers.AddVector(coordinates, (1, 1));
        }
        else
        {
            standardMove = GameHelpers.AddVector(coordinates, (0, -1));
            doubleMove = GameHelpers.AddVector(coordinates, (0, -2));
            captureLeft = GameHelpers.AddVector(coordinates, (-1, -1));
            captureRight = GameHelpers.AddVector(coordinates, (1, -1));
        }

        if (GameHelpers.IsOnBoard(standardMove) && (m_board.GetPiece(standardMove) is null))
        {
            movementSquares.Add(standardMove);
        }
        if (GameHelpers.IsOnBoard(standardMove)
            && GameHelpers.IsOnBoard(doubleMove)
            && (m_board.GetPiece(standardMove) is null)
            && (m_board.GetPiece(doubleMove) is null))
        {
            if ((m_owner == Player.White && m_row < 2)
                || (m_owner == Player.Black && m_row > 5))
            {
                movementSquares.Add(doubleMove);
            }
        }
        var occupant = m_board.GetPiece(captureLeft);
        if (GameHelpers.IsOnBoard(captureLeft)
            && ((occupant is not null) && (occupant.GetPlayer() == GameHelpers.GetOpponent(m_owner)))
            || (captureLeft == m_board.m_enPassant))
        {
            movementSquares.Add(captureLeft);
        }
        occupant = m_board.GetPiece(captureRight);
        if (GameHelpers.IsOnBoard(captureRight)
            && ((occupant is not null) && (occupant.GetPlayer() == GameHelpers.GetOpponent(m_owner)))
            || (captureRight == m_board.m_enPassant))
        {
            movementSquares.Add(captureRight);
        }

        return movementSquares;
    }

    /// <summary>
    /// Attempt to move the piece to a new square. Initiates a capture if required
    /// </summary>
    /// <param name="coordinate">The coordinate to which the piece will try to move</param>
    /// <exception cref="ArgumentException">Thrown the piece is unable to move to the specified coordinate</exception>
    public override void MovePiece((int, int) coordinate)
    {
        if (GetMovementSquares().Contains(coordinate))
        {
            if (m_board.m_game is not null)
            {
                m_board.m_game.ClearVisitedPositions();
            }
            m_board.m_halfmoveClock = 0;

            if (m_board.GetPiece(coordinate) is not null)
            {
                m_board.Capture(this, coordinate);
            }
            else if (coordinate == m_board.m_enPassant) // Capture en passant
            {
                var captureCoordinate = coordinate;
                if (m_owner == Player.White)
                {
                    // The captured piece must be 1 tile lower than the capture square
                    captureCoordinate = GameHelpers.AddVector(captureCoordinate, (0, -1));
                }
                else
                {
                    // The captured piece must be 1 tile above than the capture square
                    captureCoordinate = GameHelpers.AddVector(captureCoordinate, (0, 1));
                }
                m_board.Capture(this, captureCoordinate);
            }

            // Handle 2-square movement (enables en passant)
            if (coordinate == GameHelpers.AddVector(this.GetCoordinates(), (0, 2)))
            {
                m_board.m_enPassant = GameHelpers.AddVector(this.GetCoordinates(), (0, 1));
            }
            else if (coordinate == GameHelpers.AddVector(this.GetCoordinates(), (0, -2)))
            {
                m_board.m_enPassant = GameHelpers.AddVector(this.GetCoordinates(), (0, -1));
            }
            else
            {
                // If not 2-square movement, disable en passant
                if (m_board.m_enPassant is not null)
                {
                    m_board.m_enPassant = null;
                }
            }

            // Actually move the piece
            m_column = coordinate.Item1;
            m_row = coordinate.Item2;

            if ((m_row == GameHelpers.k_boardHeight - 1 && m_owner == Player.White)
                || (m_row == 0 && m_owner == Player.Black))
            {
                m_board.RequestPromotion(this);
            }
        }
        else
        {
            throw new ArgumentException($"Error. Piece {this.ToString()} on {GameHelpers.GetSquareFromCoordinate(GetCoordinates())} is unable to move to {GameHelpers.GetSquareFromCoordinate(coordinate)}!");
        }
    }

    public override string ToString()
    {
        return (m_owner == Player.White) ? "P" : "p";
    }
}