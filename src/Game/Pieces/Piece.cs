namespace ChessBoom.GameBoard
{
    public abstract class Piece {
        
        private Player owner;
        private int m_row;
        private int m_column;

        public Piece(Player player, int row, int column) {
            owner = player;
            m_row = row;
            m_column = column;
        }


        public abstract bool canMoveToSquare(string squareName);
    }
}