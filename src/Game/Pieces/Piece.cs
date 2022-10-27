using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    public abstract class Piece
    {
        protected Board m_board;
        protected Player m_owner;
        protected int m_row;
        protected int m_column;

        public Piece(Board board, Player player, int row, int column)
        {
            m_board = board;
            m_owner = player;
            m_row = row;
            m_column = column;
        }

        public (int, int) GetCoordinates()
        {
            return (m_row, m_column);
        }

        public abstract List<(int, int)> GetMovementSquares();

        public bool CanMoveToSquare(string squareName)
        {
            return GetMovementSquares().Contains(GameHelpers.GetCoordinateFromSquare(squareName));
        }

        public Board GetBoard()
        {
            return m_board;
        }

        public Player GetPlayer()
        {
            return m_owner;
        }
    }
}