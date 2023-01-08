using System.Collections.Generic;

namespace ChessBoom.Models.Game.Pieces;

public class Knight : Piece
{
    public Knight(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
    {
    }

    public override List<(int, int)> GetMovementSquares()
    {
        var movementSquares = new List<(int, int)>();

        var tryCoordinates = new List<(int, int)>();
        var coordinates = GetCoordinates();
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-2, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-2, 1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, -2)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, 2)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, -2)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, 2)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (2, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (2, 1)));

        foreach (var coordinate in tryCoordinates)
        {
            if (!GameHelpers.IsOnBoard(coordinate))
            {
                continue;
            }

            var occupant = m_board.GetPiece(coordinate);
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