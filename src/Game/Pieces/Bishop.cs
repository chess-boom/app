namespace ChessBoom.GameBoard
{
    public class Bishop : Piece
    {
        public Bishop(Player player, int row, int column) : base(player, row, column)
        {
        }

        public override bool CanMoveToSquare(string squareName)
        {
            return true;
        }

        public override string ToString()
        {
            return (m_owner == Player.White) ? "B" : "b";
        }
    }
}