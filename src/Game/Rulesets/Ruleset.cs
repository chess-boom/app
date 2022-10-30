namespace ChessBoom.GameBoard
{
    public abstract class Ruleset
    {
        /// <summary>
        /// Handle a capture by a piece on a specific square according to the ruleset
        /// </summary>
        /// <param name="attacker">The piece that initiates the capture sequence</param>
        /// <param name="board">The board on which the capture takes place</param>
        /// <param name="square">The square on which the capture takes place</param>
        public abstract void Capture(Piece attacker, Board board, string square);
    }
}