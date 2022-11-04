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

        /// <summary>
        /// Check if a player is in check or not, according to the ruleset
        /// </summary>
        /// <param name="player">The player who may or may not be in check</param>
        /// <param name="board">The board on which checks are searched for</param>
        /// <exception cref="ArgumentException">Thrown if a player's king is out of bounds</exception>
        public abstract bool IsInCheck(Player player, Board board);

        /// <summary>
        /// Check if a player can legally castle to a side
        /// </summary>
        /// <param name="board">The board on which the castling might take place</param>
        /// <param name="player">The player who wants to castle</param>
        /// <param name="side">The side to which the player wants to castle</param>
        public abstract bool CanCastle(Board board, Player player, Castling side);

        /// <summary>
        /// Castle a player's king
        /// </summary>
        /// <param name="game">The game in which the castling might take place</param>
        /// <param name="player">The player who wants to castle</param>
        /// <param name="side">The side to which the player wants to castle</param>
        /// <exception cref="ArgumentException">Thrown if castling is illegal</exception>
        public abstract void Castle(Game game, Player player, Castling side);

        /// <summary>
        /// Get the initial square for a rook
        /// </summary>
        /// <param name="player">The player whose rook is accessed</param>
        /// <param name="side">The rook to select</param>
        public abstract string GetInitialRookSquare(Player player, Castling side);
    }
}