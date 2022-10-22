namespace ChessBoom.GameBoard
{
    public class Bishop : Piece{
        public Bishop(Player player, int row, int column) : base(player, row, column) {
        }

        public override bool canMoveToSquare(string squareName) {
            return true;
        }
    }
}