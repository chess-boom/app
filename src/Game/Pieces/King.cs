namespace ChessBoom.GameBoard
{
    public class King : Piece{

        public King(Player player, int row, int column) : base(player, row, column) {
        }

        public override bool canMoveToSquare(string squareName) {
            return true;
        }
    }
}