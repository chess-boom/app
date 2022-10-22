namespace ChessBoom.GameBoard
{
    public class Queen : Piece{

        public Queen(Player player, int row, int column) : base(player, row, column) {
        }

        public override bool canMoveToSquare(string squareName) {
            return true;
        }
    }
}