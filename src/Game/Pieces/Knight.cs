namespace ChessBoom.GameBoard
{
    public class Knight : Piece{

        public Knight(Player player, int row, int column) : base(player, row, column) {
        }

        public override bool canMoveToSquare(string squareName) {
            return true;
        }
    }
}