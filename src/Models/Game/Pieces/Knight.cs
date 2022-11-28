using System.Collections.Generic;

namespace ChessBoom.Models.Game;

public class Knight : Piece
{
    public Knight(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
    {
    }

    public override List<(int, int)> GetMovementSquares()
    {
        List<(int, int)> movementSquares = new List<(int, int)>();

        List<(int, int)> tryCoordinates = new List<(int, int)>();
        (int, int) coordinates = GetCoordinates();
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-2, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-2, 1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, -2)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, 2)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, -2)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, 2)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (2, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (2, 1)));

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

    public override string ToString()
    {
        return (m_owner == Player.White) ? "N" : "n";
    }
}