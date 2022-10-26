using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    public class Bishop : Piece
    {
        public Bishop(Board board, Player player, int row, int column) : base(board, player, row, column)
        {
        }

        public override List<(int, int)> GetMovementSquares()
        {
            List<(int, int)> movementSquares = new List<(int, int)>();
            (int, int) movementVector;
            (int, int) position = (m_column, m_row);

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

        public override bool CanMoveToSquare(string squareName)
        {
            return true;
        }

        public override string ToString()
        {
            return (m_owner == Player.White) ? "B" : "b";
        }
    }
}