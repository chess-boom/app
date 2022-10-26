using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    public class Knight : Piece
    {
        public Knight(Board board, Player player, int row, int column) : base(board, player, row, column)
        {
        }

        public override List<(int, int)> GetMovementSquares()
        {
            return new List<(int, int)>();
        }

        public override bool CanMoveToSquare(string squareName)
        {
            return true;
        }

        public override string ToString()
        {
            return (m_owner == Player.White) ? "N" : "n";
        }
    }
}