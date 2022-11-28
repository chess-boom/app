using System;
using System.Collections.Generic;
using System.IO;
using ChessBoom.Models.Game.Pieces;
using ChessBoom.Models.Game.Rulesets;

namespace ChessBoom.Models.Game;

enum Variant
{
    Standard,
    Chess960,
    Atomic,
    Horde
}

public enum Player
{
    White,
    Black
}

public enum Castling
{
    Kingside,
    Queenside
}

public enum GameState
{
    // TODO: Create a game loop
    Setup,
    InProgress,
    VictoryWhite,
    VictoryBlack,
    // TODO: Implement draw checks
    Draw,
    // TODO: Implement game aborting
    Aborted
}

/// <summary>
/// The Move struct represents a single move and all of its variations via a nested recursive list
/// </summary>
struct Move
{
    /// <summary>
    /// The notation for kingside castling
    /// </summary>
    public const string k_kingsideCastleNotation = "O-O";
    /// <summary>
    /// The notation for kingside castling
    /// </summary>
    public const string k_queensideCastleNotation = "O-O-O";
    public Move(Piece piece, string square)
    {
        m_piece = piece;
        m_square = square;
        m_variations = null;
    }
    public Piece m_piece { get; }
    public string m_square { get; }
    public List<Move>? m_variations { get; set; }

    // TODO: Implement variations
    public void AddVariation(Move move)
    {
        if (m_variations is null)
        {
            m_variations = new List<Move>();
        }
        if (!m_variations.Contains(move))
        {
            m_variations.Add(move);
        }
        else
        {
            throw new ArgumentException($"Variation {move} already exists");
        }
    }

    public override string ToString()
    {
        var pieceString = m_piece.ToString();
        return pieceString?.ToLower() + m_square;
    }
}

/// <summary>
/// The GameplayErrorException class is used for any case in which gameplay rules are broken
/// </summary>
public class GameplayErrorException : Exception
{
    public GameplayErrorException()
    {
    }

    public GameplayErrorException(string message) : base(message)
    {
    }

    public GameplayErrorException(string message, Exception inner) : base(message, inner)
    {
    }
}

/// <summary>
/// The Game class handles the creation and playing of a game of any chess variant
/// </summary>
public class Game
{
    /// <summary>
    /// The chosen variant for this game
    /// </summary>
    private readonly Variant m_variant = Variant.Standard;
    /// <summary>
    /// The chosen ruleset for this game
    /// </summary>
    public Ruleset m_ruleset { get; set; }
    /// <summary>
    /// The board created for this game
    /// </summary>
    public Board m_board { get; set; }
    /// <summary>
    /// The data structure for all moves and variations
    /// </summary>
    private readonly List<Move> m_moveList;
    /// <summary>
    /// Maps repeatable unique board positions to the number of times they have been reached
    /// </summary>
    private Dictionary<string, int> m_visitedPositions;

    /// <summary>
    /// The present game state
    /// </summary>
    public GameState m_gameState { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public Game()
    {
        m_board = InitializeBoard(m_variant);
        m_ruleset = Standard.Instance;
        m_moveList = new List<Move>();
        m_visitedPositions = new Dictionary<string, int>();
        m_gameState = GameState.InProgress;
    }

    /*public Game(Variant variant)
    {
        m_variant = variant;
        Game();
    }*/

    /// <summary>
    /// The board object is created and initialized
    /// </summary>
    /// <param name="variant">The chosen variant for the board</param>
    private Board InitializeBoard(Variant variant)
    {
        var fen = "";

        // Note: Standard and Atomic use the default board. Chess960 and Horde use different initial configurations
        switch (variant)
        {
            case Variant.Chess960:
                // TODO: CB-24
                break;
            case Variant.Horde:
                fen = File.ReadAllText("Resources/horde.fen");
                break;
            case Variant.Standard:
            case Variant.Atomic:
            default:
                fen = File.ReadAllText("Resources/default.fen");
                break;
        }

        return CreateBoardFromFEN(this, fen);
    }

    /// <summary>
    /// Handle the capture that has occurred on a specific square
    /// </summary>
    /// <param name="attacker">The piece that initiated the capture</param>
    /// <param name="coordinate">The square on which the capture takes place</param>
    /// <exception cref="ArgumentException">Thrown the piece on the starting square can not be found or be moved, or the square can not be found</exception>
    public void MakeExplicitMove(string startingSquare, string destinationSquare)
    {
        var piece = m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(startingSquare));
        if (piece is null)
        {
            throw new ArgumentException($"Piece on square {startingSquare} not found!");
        }
        MakeMove(piece, destinationSquare);
    }

    /// <summary>
    /// Attempt to move a piece as a player's move
    /// </summary>
    /// <param name="piece">The piece that will attempt to move</param>
    /// <param name="square">The square that the piece should move to</param>
    /// <exception cref="ArgumentException">Thrown the piece can not be found or be moved, or the square can not be found</exception>
    /// <exception cref="GameplayErrorException">Thrown if the wrong player attempts to make a move or if castling is attempted when illegal</exception>
    public void MakeMove(Piece piece, string square)
    {
        if (m_gameState != GameState.InProgress)
        {
            throw new GameplayErrorException($"Game is not in progress! Illegal move.");
        }
        if (piece.GetPlayer() != m_board.m_playerToPlay)
        {
            throw new GameplayErrorException($"Piece {piece} can not move because it is not {piece.GetPlayer()}\'s turn!");
        }
        m_board.m_halfmoveClock++;

        var legacyBoard = CreateBoardFromFEN(this, CreateFENFromBoard(m_board));

        if (square == Move.k_kingsideCastleNotation || square == Move.k_queensideCastleNotation)
        {
            m_ruleset.Castle(m_board, piece.GetPlayer(), (square == Move.k_kingsideCastleNotation) ? Castling.Kingside : Castling.Queenside);
        }
        else
        {
            piece.MovePiece(GameHelpers.GetCoordinateFromSquare(square));
        }

        if (m_board.m_playerToPlay == Player.Black)
        {
            m_board.m_fullmoveCount++;
        }
        m_board.m_playerToPlay = GameHelpers.GetOpponent(m_board.m_playerToPlay);

        if (m_ruleset.IsIllegalBoardState(m_board))
        {
            m_board = legacyBoard;
            throw new GameplayErrorException("Error! Illegal move!");
        }

        var boardPosition = String.Format("{0} {1} {2} {3}",
            GetPiecesFENFromBoard(m_board),
            GetPlayerFENFromBoard(m_board),
            m_board.GetCastling(),
            GetEnPassantFENFromBoard(m_board));

        if (m_visitedPositions.ContainsKey(boardPosition))
        {
            m_visitedPositions[boardPosition]++;
        }
        else
        {
            m_visitedPositions.Add(boardPosition, 1);
        }
        m_moveList.Add(new Move(piece, square));

        m_ruleset.AssessBoardState(this, m_board);
    }

    /// <summary>
    /// Event that all previous positions may no longer be re-reached
    /// </summary>
    public void ClearVisitedPositions()
    {
        m_visitedPositions.Clear();
    }

    /// <summary>
    /// Determine whether or not threefold repetition has been reached
    /// </summary>
    /// <returns>If threefold repetition has occurred</returns>
    public bool HasThreefoldRepetition()
    {
        foreach (var visitedPosition in m_visitedPositions)
        {
            if (visitedPosition.Value >= Ruleset.k_threefoldRepetitionCount)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// The board is created and populated from a .FEN file.
    /// FEN files have 6 parts, delimited by whitespace characters:
    ///     The first part is the piece placements, rows delimited by '/' characters starting on the top.
    ///     The second part denotes the next player to take their turn.
    ///     The third part denotes castling availability.
    ///     The fourth part denotes en passant availability.
    ///     The fifth part denotes the halfmove clock, useful for enforcing the fifty-move rule.
    ///     The sixth part denotes the fullmove number.
    /// </summary>
    /// <param name="fen">The contents of the .FEN file</param>
    public static Board CreateBoardFromFEN(Game game, string fen)
    {
        var board = new Board(game);
        var fenSplit = fen.Split(' ');

        board.CreateBoard(fenSplit[0]);

        // Set the next player to play
        if (fenSplit[1].Length != 1)
        {
            throw new ArgumentException($"Player \"{fenSplit[1]}\" does not denote a single player.");
        }
        try
        {
            board.SetPlayerToPlay(fenSplit[1][0]);
        }
        catch (ArgumentException)
        {
            board.SetPlayerToPlay('w');
        }

        // Set castling availability
        try
        {
            board.SetCastling(fenSplit[2]);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine($"{e}: defaulting to no castling rights");
            board.SetCastling("-");
        }

        // Set en passant capability
        if (fenSplit[3] != "-")
        {
            try
            {
                board.m_enPassant = GameHelpers.GetCoordinateFromSquare(fenSplit[3]);
            }
            catch (ArgumentException)
            {
                board.m_enPassant = null;
            }
        }
        else
        {
            board.m_enPassant = null;
        }

        // Set halfmove clock
        try
        {
            board.m_halfmoveClock = int.Parse(fenSplit[4]);
        }
        catch (FormatException)
        {
            board.m_halfmoveClock = 0;
        }

        // Set fullmove number
        try
        {
            board.m_fullmoveCount = int.Parse(fenSplit[5]);
        }
        catch (FormatException)
        {
            board.m_fullmoveCount = 0;
        }

        return board;
    }

    /// <summary>
    /// Retrieve the board state as the contents of a .FEN file
    /// </summary>
    /// <returns>The board state as the contents of a .FEN file</returns>
    public static string CreateFENFromBoard(Board board)
    {
        return String.Format("{0} {1} {2} {3} {4} {5}",
            GetPiecesFENFromBoard(board),
            GetPlayerFENFromBoard(board),
            board.GetCastling(),
            GetEnPassantFENFromBoard(board),
            board.m_halfmoveClock.ToString(),
            board.m_fullmoveCount.ToString());
    }

    /// <summary>
    /// Retrieve the board state of the pieces as the contents of a .FEN file
    /// </summary>
    /// <returns>The board state of the pieces as the contents of a .FEN file</returns>
    private static string GetPiecesFENFromBoard(Board board)
    {
        var fen = "";

        for (var row = GameHelpers.k_boardHeight - 1; row >= 0; row--)
        {
            var emptySquareCount = 0;
            for (var col = 0; col < GameHelpers.k_boardWidth; col++)
            {
                var piece = board.GetPiece((col, row));
                if (piece is null)
                {
                    emptySquareCount++;
                    continue;
                }
                else
                {
                    // Append the number of empty squares and reset the count
                    if (emptySquareCount != 0)
                    {
                        fen += emptySquareCount.ToString();
                        emptySquareCount = 0;
                    }

                    fen += piece.ToString();
                }
            }

            if (emptySquareCount != 0)
            {
                fen += emptySquareCount.ToString();
                emptySquareCount = 0;
            }

            if (row != 0)
            {
                fen += "/";
            }
        }

        return fen;
    }

    /// <summary>
    /// Retrieve the next player as the contents of a .FEN file
    /// </summary>
    /// <returns>The next player as the contents of a .FEN file</returns>
    private static string GetPlayerFENFromBoard(Board board)
    {
        return (board.m_playerToPlay == Player.White) ? "w" : "b";
    }

    /// <summary>
    /// Retrieve the en passant availability as the contents of a .FEN file
    /// </summary>
    /// <returns>The en passant availability as the contents of a .FEN file</returns>
    private static string GetEnPassantFENFromBoard(Board board)
    {
        try
        {
            if (board.m_enPassant.HasValue)
            {
                return GameHelpers.GetSquareFromCoordinate(board.m_enPassant.Value);
            }
            else
            {
                return "-";
            }
        }
        catch (ArgumentException)
        {
            return "-";
        }
    }
}