using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ChessBoom.Models.Game.Pieces;
using ChessBoom.Models.Game.Rulesets;

namespace ChessBoom.Models.Game;

public enum Variant
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
    InProgress,
    VictoryWhite,
    VictoryBlack,
    Draw,
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
[System.SerializableAttribute()] // Used to conform to the ISerializable interface.
public class GameplayErrorException : Exception
{

    public GameplayErrorException() { }

    public GameplayErrorException(string message) : base(message) { }

    public GameplayErrorException(string message, Exception inner) : base(message, inner) { }
    // This constructor is needed for serialization. Used to conform to the ISerializable interface.
    protected GameplayErrorException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

/// <summary>
/// The Game class handles the creation and playing of a game of any chess variant
/// </summary>
public class Game
{

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
    public Game(Variant variant = Variant.Standard)
    {
        m_board = InitializeBoard(variant);
        m_ruleset = Ruleset.k_rulesetUsage[variant];
        m_moveList = new List<Move>();
        m_visitedPositions = new Dictionary<string, int>();
        m_gameState = GameState.InProgress;
    }

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
    /// Make a move using PGN notation format
    /// </summary>
    /// <param name="pgnNotation">The PGN notation representing the move</param>
    /// <exception cref="ArgumentException">Thrown if the PGN notation does not denote a unique, valid move</exception>
    public void MakePGNMove(string pgnNotation)
    {
        // Castling
        if (pgnNotation == Move.k_kingsideCastleNotation
            || pgnNotation == Move.k_queensideCastleNotation)
        {
            MakeMove(m_ruleset.GetKing(m_board, m_board.m_playerToPlay), pgnNotation);
            return;
        }

        // Gather extra data
        List<string> rowInstances = new List<string>();
        List<string> colInstances = new List<string>();
        foreach (char c in pgnNotation)
        {
            if (GameHelpers.k_boardColumnNames.Contains(c.ToString()))
            {
                colInstances.Add(c.ToString());
                continue;
            }
            if (GameHelpers.k_boardRowNames.Contains(c.ToString()))
            {
                rowInstances.Add(c.ToString());
                continue;
            }
        }

        bool containsCapture = (pgnNotation.IndexOf('x') != -1);
        bool containsCheck = (pgnNotation.IndexOf('+') != -1);
        bool containsCheckmate = (pgnNotation.IndexOf('#') != -1);
        bool containsPromotion = (pgnNotation.IndexOf('=') != -1);

        // Get the target square
        Board dummyBoard = new Board();
        string square = colInstances[colInstances.Count - 1] + rowInstances[rowInstances.Count - 1];
        (int, int) squareCoordinates = GameHelpers.GetCoordinateFromSquare(square);

        // Get piece type
        Piece dummyPiece = null!;

        // Consider reimplementing like Board's k_pieceConstructor
        List<(int, int)> possibleOrigins;
        switch (pgnNotation[0])
        {
            case 'Q':
                dummyPiece = new Queen(dummyBoard, m_board.m_playerToPlay, squareCoordinates);
                possibleOrigins = dummyPiece.GetMovementSquares();
                break;
            case 'K':
                dummyPiece = new King(dummyBoard, m_board.m_playerToPlay, squareCoordinates);
                possibleOrigins = dummyPiece.GetMovementSquares();
                break;
            case 'R':
                dummyPiece = new Rook(dummyBoard, m_board.m_playerToPlay, squareCoordinates);
                possibleOrigins = dummyPiece.GetMovementSquares();
                break;
            case 'N':
                dummyPiece = new Knight(dummyBoard, m_board.m_playerToPlay, squareCoordinates);
                possibleOrigins = dummyPiece.GetMovementSquares();
                break;
            case 'B':
                dummyPiece = new Bishop(dummyBoard, m_board.m_playerToPlay, squareCoordinates);
                possibleOrigins = dummyPiece.GetMovementSquares();
                break;
            default:
                dummyPiece = new Pawn(dummyBoard, m_board.m_playerToPlay, squareCoordinates);
                possibleOrigins = ((Pawn)dummyPiece).GetPossibleOriginSquares();
                break;
        }

        // Get the piece
        List<Piece> possiblePieces = new List<Piece>();
        foreach ((int, int) originCoordinate in possibleOrigins)
        {
            Piece? candidatePiece = m_board.GetPiece(originCoordinate);
            if (candidatePiece is not null)
            {
                possiblePieces.Add(candidatePiece);
            }
        }
        // Get correct piece
        Piece correctPiece = null!;
        foreach (Piece candidatePiece in possiblePieces)
        {
            if (candidatePiece.GetType() != dummyPiece.GetType())
            {
                continue;
            }
            if (!candidatePiece.GetLegalMoves().Contains(square))
            {
                continue;
            }
            if (candidatePiece.GetPlayer() != m_board.m_playerToPlay)
            {
                continue;
            }
            if (rowInstances.Count == 1 && colInstances.Count == 1)
            {
                correctPiece = candidatePiece;
                break;
            }
            if (rowInstances.Count != 1 && candidatePiece.GetCoordinates().Item2 != GameHelpers.k_boardRowNames.IndexOf(rowInstances[0]))
            {
                continue;
            }
            if (colInstances.Count != 1 && candidatePiece.GetCoordinates().Item1 != GameHelpers.k_boardColumnNames.IndexOf(colInstances[0]))
            {
                continue;
            }
            correctPiece = candidatePiece;
            break;
        }

        MakeMove(correctPiece, square);
    }

    /// <summary>
    /// Make a move from two specified squares
    /// </summary>
    /// <param name="startingSquare">The square from which a piece moves</param>
    /// <param name="destinationSquare">The square to which a piece moves</param>
    /// <param name="requestPromotionPiece">Optional parameter denoting the function to call to determine promotion piece</param>
    /// <exception cref="ArgumentException">Thrown the piece on the starting square can not be found or be moved, or the square can not be found</exception>
    /// <exception cref="GameplayErrorException">Thrown if the attempted move is invalid as per gameplay rules</exception>
    public void MakeExplicitMove(string startingSquare, string destinationSquare, Board.RequestPromotionPieceDelegate? requestPromotionPiece = null)
    {
        var piece = m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(startingSquare));
        if (piece is null)
        {
            throw new ArgumentException($"Piece on square {startingSquare} not found!");
        }

        MakeMove(piece, destinationSquare, requestPromotionPiece);
    }

    /// <summary>
    /// Attempt to move a piece as a player's move
    /// </summary>
    /// <param name="piece">The piece that will attempt to move</param>
    /// <param name="square">The square that the piece should move to</param>
    /// <param name="requestPromotionPiece">Optional parameter denoting the function to call to determine promotion piece</param>
    /// <exception cref="ArgumentException">Thrown the piece can not be found or be moved, or the square can not be found</exception>
    /// <exception cref="GameplayErrorException">Thrown if the wrong player attempts to make a move or if castling is attempted when illegal</exception>
    public void MakeMove(Piece piece, string square, Board.RequestPromotionPieceDelegate? requestPromotionPiece = null)
    {
        if (m_gameState != GameState.InProgress)
        {
            throw new GameplayErrorException("Game is not in progress! Illegal move.");
        }

        if (piece.GetPlayer() != m_board.m_playerToPlay)
        {
            throw new GameplayErrorException(
                $"Piece {piece} can not move because it is not {piece.GetPlayer()}\'s turn!");
        }

        m_board.m_halfmoveClock++;

        var legacyBoard = CreateBoardFromFEN(this, CreateFENFromBoard(m_board));

        if (square is Move.k_kingsideCastleNotation or Move.k_queensideCastleNotation)
        {
            m_ruleset.Castle(m_board, piece.GetPlayer(),
                (square == Move.k_kingsideCastleNotation) ? Castling.Kingside : Castling.Queenside);
        }
        else
        {
            piece.MovePiece(GameHelpers.GetCoordinateFromSquare(square), requestPromotionPiece);
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
    /// <param name="game">The game which the resultant Board will correspond to</param>
    /// <param name="fen">The contents of the .FEN file</param>
    public static Board CreateBoardFromFEN(Game? game, string fen)
    {
        Board board;
        if (game is not null)
        {
            board = new Board(game);
        }
        else
        {
            board = new Board();
        }
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
        StringBuilder fen = new StringBuilder();

        for (var row = GameHelpers.k_boardHeight - 1; row >= 0; row--)
        {
            var emptySquareCount = 0;
            for (var col = 0; col < GameHelpers.k_boardWidth; col++)
            {
                var piece = board.GetPiece((col, row));
                if (piece is null)
                {
                    emptySquareCount++;
                }
                else
                {
                    // Append the number of empty squares and reset the count
                    if (emptySquareCount != 0)
                    {
                        fen.Append(emptySquareCount);
                        emptySquareCount = 0;
                    }

                    fen.Append(piece.ToString());
                }
            }

            if (emptySquareCount != 0)
            {
                fen.Append(emptySquareCount);
            }

            if (row != 0)
            {
                fen.Append('/');
            }
        }

        return fen.ToString();
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

            return "-";
        }
        catch (ArgumentException)
        {
            return "-";
        }
    }
}