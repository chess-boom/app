namespace ChessBoom.GameBoard
{
    public class Knight : Piece{

        public Knight(Player player, int row, int column) : base(player, row, column) {
        }

        public override bool CanMoveToSquare(string squareName) {
            return true;
        }

        public override string ToString()
        {
            return (m_owner == Player.White) ? "N" : "n";
        }
    }
}