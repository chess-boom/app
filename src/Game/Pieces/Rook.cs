using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    public class Rook : Piece
    {
        public Rook(Board board, Player player, int row, int column) : base(board, player, row, column)
        {
        }

        public override List<(int, int)> GetMovementSquares()
        {
            List<(int, int)> movementSquares = new List<(int, int)>();
            (int, int) movementVector;
            (int, int) position = GetCoordinates();

            // Top
            movementVector = (0, 1);
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

        public override string ToString()
        {
            return (m_owner == Player.White) ? "R" : "r";
        }
    }
}