namespace ChessBoom.GameBoard
{
    public class Pawn : Piece{

        public Pawn(Player player, int row, int column) : base(player, row, column) {
        }

        public override bool canMoveToSquare(string squareName) {
            return true;
        }
    }
}