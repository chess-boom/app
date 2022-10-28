namespace ChessBoom.GameBoard
{
    public abstract class Ruleset
    {
        public abstract void Capture(Piece attacker, Board board, string square);
    }
}