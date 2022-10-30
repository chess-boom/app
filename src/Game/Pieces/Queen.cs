using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    public class Queen : Piece
    {
        public Queen(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
        {
        }

        public override List<(int, int)> GetMovementSquares()
        {
            List<(int, int)> movementSquares = new List<(int, int)>();
            (int, int) movementVector;
            (int, int) position = GetCoordinates();

            // Top left
            movementVector = (-1, 1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Top
            movementVector = (0, 1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Top right
            movementVector = (1, 1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Left
            movementVector = (-1, 0);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Right
            movementVector = (1, 0);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Bottom left
            movementVector = (-1, -1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Bottom
            movementVector = (0, -1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Bottom right
            movementVector = (1, -1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);

            return movementSquares;
        }

        public override string ToString()
        {
            return (m_owner == Player.White) ? "Q" : "q";
        }
    }
}