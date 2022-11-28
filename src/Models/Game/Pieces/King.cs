using System.Collections.Generic;

namespace ChessBoom.Models.Game.Pieces;

public class King : Piece
{
    /// <summary>
    /// Flag to keep track of if the king has moved
    /// </summary>
    private bool m_hasMoved;
    public King(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
    {
        m_hasMoved = false;
    }

    public override List<(int, int)> GetMovementSquares()
    {
        List<(int, int)> movementSquares = new List<(int, int)>();

        List<(int, int)> tryCoordinates = new List<(int, int)>();
        (int, int) coordinates = GetCoordinates();
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, 0)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, 1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (0, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (0, 1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, 0)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, 1)));

        foreach ((int, int) coordinate in tryCoordinates)
        {
            if (!GameHelpers.IsOnBoard(coordinate))
            {
                continue;
            }
            Piece? occupant = m_board.GetPiece(coordinate);
            if (occupant is not null && occupant.GetPlayer() == m_owner)
            {
                continue;
            }

            movementSquares.Add(coordinate);
        }
        return movementSquares;
    }

    public override void MovePiece((int, int) coordinate)
    {
        base.MovePiece(coordinate);
        if (!m_hasMoved)
        {
            m_board.RemoveCastling(m_owner, Castling.Kingside);
            m_board.RemoveCastling(m_owner, Castling.Queenside);
            m_hasMoved = true;
        }
    }

    public override string ToString()
    {
        return (m_owner == Player.White) ? "K" : "k";
    }
}