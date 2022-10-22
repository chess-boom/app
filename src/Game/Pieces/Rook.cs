namespace ChessBoom.GameBoard
{
    public class Rook : Piece{

        public Rook(Player player, int row, int column) : base(player, row, column) {
        }

        public override bool canMoveToSquare(string squareName) {
            return true;
        }
    }
}