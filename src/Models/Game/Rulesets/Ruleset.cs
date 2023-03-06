using System;
using System.Collections.Generic;
using ChessBoom.Models.Game.Pieces;

namespace ChessBoom.Models.Game.Rulesets;

public abstract class Ruleset
{
    /// <summary>
    /// The number of repetitions for threefold repetition to occur
    /// </summary>
    public const int k_threefoldRepetitionCount = 3;

    /// <summary>
    /// The limiting number of moves that amount to "no progress" before a game ends in a draw
    /// </summary>
    public const int k_progressMoveLimit = 50;

    /// <summary>
    /// Map for piece types and their constructors
    /// </summary>
    public static readonly Dictionary<Variant, Ruleset> k_rulesetUsage = new()
    {
        // TODO: Update when more rulesets are added
        { Variant.Standard, Standard.Instance },
        { Variant.Chess960, Standard.Instance },
        { Variant.Atomic, Atomic.Instance },
        { Variant.Horde, Standard.Instance }
    };

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
    /// <param name="board">The board in which the castling might take place</param>
    /// <param name="player">The player who wants to castle</param>
    /// <param name="side">The side to which the player wants to castle</param>
    /// <exception cref="ArgumentException">Thrown if castling is illegal</exception>
    public abstract void Castle(Board board, Player player, Castling side);

    /// <summary>
    /// Get the initial square for a rook
    /// </summary>
    /// <param name="player">The player whose rook is accessed</param>
    /// <param name="side">The rook to select</param>
    /// <returns>The name of the square that the rook initially stands upon</returns>
    public abstract string GetInitialRookSquare(Player player, Castling side);

    /// <summary>
    /// Determine if a specified board contains an illegal state
    /// </summary>
    /// <param name="board">The board to assess</param>
    /// <returns>Whether or not the board is legal as per the ruleset</returns>
    public abstract bool IsIllegalBoardState(Board board);

    /// <summary>
    /// Assess the state of the specified board. Covers victory/defeat conditions.
    /// </summary>
    /// <param name="game">The game, whose state may change</param>
    /// <param name="board">The board to assess</param>
    public abstract void AssessBoardState(Game game, Board board);

    /// <summary>
    /// TODO
    /// </summary>
    public abstract Piece GetKing(Board board, Player player);
}