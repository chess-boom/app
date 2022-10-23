namespace ChessBoom.GameBoard
{
    public abstract class Piece {
        
        protected Player m_owner;
        protected int m_row;
        protected int m_column;

        public Piece(Player player, int row, int column) {
            m_owner = player;
            m_row = row;
            m_column = column;
        }

        public (int, int) GetCoordinates() {
            return (m_row, m_column);
        }
        public abstract bool CanMoveToSquare(string squareName);
    }
}