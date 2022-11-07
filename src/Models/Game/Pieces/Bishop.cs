using System.Collections.Generic;

namespace ChessBoom.Views.GameBoard
{
    public class Bishop : Piece
    {
        public Bishop(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
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
            // Top right
            movementVector = (1, 1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Bottom left
            movementVector = (-1, -1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Bottom right
            movementVector = (1, -1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);

            return movementSquares;
        }

        public override string ToString()
        {
            return (m_owner == Player.White) ? "B" : "b";
        }
    }
}