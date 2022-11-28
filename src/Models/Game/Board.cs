using System;
using System.Text;
using System.Collections.Generic;
using ChessBoom.Models.Game.Pieces;
using ChessBoom.Models.Game.Rulesets;

namespace ChessBoom.Models.Game;

/// <summary>
/// The Board class contains the chess pieces and is the medium through which a Game is played
/// </summary>
public class Board
{
    /// <summary>
    /// Map for piece types and their constructors
    /// </summary>
    protected static readonly Dictionary<char, Func<Board, Player, (int, int), Piece>> k_pieceConstructor = new Dictionary<char, Func<Board, Player, (int, int), Piece>>()
    {
        {'K', (board, player, coordinate) => new King(board, player, coordinate)},
        {'Q', (board, player, coordinate) => new Queen(board, player, coordinate)},
        {'R', (board, player, coordinate) => new Rook(board, player, coordinate)},
        {'N', (board, player, coordinate) => new Knight(board, player, coordinate)},
        {'B', (board, player, coordinate) => new Bishop(board, player, coordinate)},
        {'P', (board, player, coordinate) => new Pawn(board, player, coordinate)}
    };

    protected static readonly char k_noCastling = '-';

    protected static readonly Dictionary<char, Tuple<Player?, Castling?>> k_FENToCastling = new Dictionary<char, Tuple<Player?, Castling?>>()
    {
        {'K', new Tuple<Player?, Castling?>(Player.White, Castling.Kingside)},
        {'Q', new Tuple<Player?, Castling?>(Player.White, Castling.Queenside)},
        {'k', new Tuple<Player?, Castling?>(Player.Black, Castling.Kingside)},
        {'q', new Tuple<Player?, Castling?>(Player.Black, Castling.Queenside)},
        {k_noCastling, new Tuple<Player?, Castling?>(null, null)}
    };

    protected static readonly Dictionary<Tuple<Player, Castling>, char> k_castlingToFEN = new Dictionary<Tuple<Player, Castling>, char>()
    {
        {new Tuple<Player, Castling>(Player.White, Castling.Kingside), 'K'},
        {new Tuple<Player, Castling>(Player.White, Castling.Queenside), 'Q'},
        {new Tuple<Player, Castling>(Player.Black, Castling.Kingside), 'k'},
        {new Tuple<Player, Castling>(Player.Black, Castling.Queenside), 'q'}
    };

    /// <summary>
    /// The next player to move
    /// </summary>
    public Player m_playerToPlay { get; set; } = Player.White;
    /// <summary>
    /// The ability for each player to castle
    /// </summary>
    private Dictionary<Player, List<Castling>> m_castling { get; set; }
    /// <summary>
    /// The square on which en passant may be played (if any)
    /// </summary>
    public (int, int)? m_enPassant { get; set; }
    /// <summary>
    /// The number of half-moves since the last capture or pawn advance
    /// </summary>
    public int m_halfmoveClock { get; set; } = 0;
    /// <summary>
    /// The number of full moves (starting at 1, incremented after Black moves)
    /// </summary>
    public int m_fullmoveCount { get; set; } = 0;
    /// <summary>
    /// The list of chess pieces
    /// </summary>
    public List<Piece> m_pieces { get; set; }
    /// <summary>
    /// The game that contains this board
    /// </summary>
    public Game? m_game { get; set; }

    /// <summary>
    /// The default constructor
    /// </summary>
    public Board()
    {
        m_pieces = new List<Piece>();
        m_castling = new Dictionary<Player, List<Castling>>();
    }

    /// <summary>
    /// The parameterized constructor
    /// </summary>
    public Board(Game game) : this()
    {
        m_game = game;
    }

    /// <summary>
    /// Retrieves a piece from its coordinates
    /// </summary>
    /// <param name="coordinate">The 2-tuple containing the row and column coordinates (0-7, 0-7)</param>
    /// <returns>The piece found on the passed square. If none, returns null</returns>
    public Piece? GetPiece((int, int) coordinate)
    {
        foreach (var piece in m_pieces)
        {
            if (coordinate == piece.GetCoordinates())
            {
                return piece;
            }
        }
        return null;
    }

    /// <summary>
    /// Creates a piece
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the passed player is invalid</exception>
    /// <exception cref="ArgumentException">Thrown if invalid pieceType or invalid coordinates</exception>
    public void CreatePiece(char pieceType, (int, int) coordinate)
    {
        if (!GameHelpers.IsOnBoard(coordinate))
        {
            throw new ArgumentException($"Coordinate ({coordinate.Item1}, {coordinate.Item2}) is an invalid coordinate (x, y).");
        }

        var player = Char.IsUpper(pieceType) ? Player.White : Player.Black;

        Piece piece;
        try
        {
            piece = k_pieceConstructor[Char.ToUpper(pieceType)](this, player, coordinate);
        }
        catch (KeyNotFoundException)
        {
            throw new ArgumentException($"Error. {pieceType} is an invalid piece type.");
        }

        m_pieces.Add(piece);
    }

    /// <summary>
    /// Create a board from the piece placement data of a FEN string
    /// </summary>
    /// <param name="fen">Each rank is described, starting with rank 8 and ending with rank 1, with a "/" between each one; within each rank, the contents of the squares are described in order from the a-file to the h-file.</param>
    public void CreateBoard(string fen)
    {
        var pieceSplit = fen.Split('/');
        for (var row = 0; row < GameHelpers.k_boardHeight; row++)
        {
            var col = 0;

            foreach (var piece in pieceSplit[row])
            {
                if (Char.IsDigit(piece))
                {
                    col += (int)Char.GetNumericValue(piece);
                }
                else
                {
                    try
                    {
                        (int, int) coordinate = (col, (GameHelpers.k_boardHeight - 1) - row);
                        CreatePiece(piece, coordinate);
                    }
                    catch (ArgumentException)
                    {

                    }
                    col++;
                }
            }
        }
    }

    // TODO: Determine whether this function should be kept or not
    // Note: This may be useful in the future for when variations are made.
    //      Depending on the architecture, a game may delegate each variation
    //      to its own board, which should then make its own moves.

    /// <summary>
    /// Moves a piece from one square to another
    /// </summary>
    /// <param name="start">The name of the square on which the moving piece resides</param>
    /// <param name="destination">The name of the square to which the piece will move</param>
    /// <exception cref="ArgumentException">Thrown if the specified starting square is invalid/exception>
    /// <exception cref="ArgumentException">Thrown if the specified starting square contains no piece</exception>
    /// <exception cref="ArgumentException">Thrown if the found piece is unable to move to the specified coordinate</exception>
    /*public void MovePiece(string start, string destination)
    {
        (int, int) startCoordinate;
        (int, int) destinationCoordinate;
        try
        {
            startCoordinate = GameHelpers.GetCoordinateFromSquare(start);
            destinationCoordinate = GameHelpers.GetCoordinateFromSquare(destination);
        }
        catch (ArgumentException e)
        {
            throw e;
        }

        Piece? piece = GetPiece(startCoordinate);
        if (piece is null)
        {
            throw new ArgumentException($"Square {start} has no piece to move");
        }

        // TODO: Insert additional conditions for moving pieces here
        // Ex: check, castling through check, etc.

        try
        {
            piece.MovePiece(destinationCoordinate);
        }
        catch (ArgumentException e)
        {
            throw e;
        }
    }*/

    /// <summary>
    /// Mutator for the next player to play
    /// </summary>
    /// <param name="player">The .FEN notation of the player (w, b)</param>
    /// <exception cref="ArgumentException">Thrown when the passed player is invalid</exception>
    public void SetPlayerToPlay(char player)
    {
        switch (player)
        {
            case 'w':
                m_playerToPlay = Player.White;
                break;
            case 'b':
                m_playerToPlay = Player.Black;
                break;
            default:
                throw new ArgumentException($"Player \'{player}\' is not a valid player character.");
        }
    }

    /// <summary>
    /// Accessor for the board's ruleset. Defaults to standard chess rules.
    /// </summary>
    /// <returns>The game's ruleset. Returns standard chess rules if the board does not have a game.</returns>
    public Ruleset GetRuleset()
    {
        if (m_game is not null)
        {
            return m_game.m_ruleset;
        }
        else
        {
            return Standard.Instance;
        }
    }

    /// <summary>
    /// Mutator for castling privileges
    /// </summary>
    /// <param name="castling">The .FEN notation of the castling privileges (ex: "KQkq")</param>
    /// <exception cref="ArgumentException">Thrown when the castling privileges contains an invalid character</exception>
    public void SetCastling(string castling)
    {
        m_castling = new Dictionary<Player, List<Castling>>()
        {
            {Player.White, new List<Castling>()},
            {Player.Black, new List<Castling>()},
        };

        if (castling.Length < 1)
        {
            throw new ArgumentException("FEN file must include castling rights");
        }

        foreach (var c in castling)
        {
            // Here we do 2 lookups, but unpacking in-place is awkward
            if (!k_FENToCastling.TryGetValue(c, out _))
            {
                throw new ArgumentException($"Invalid character \'{c}\' in FEN file");
            }

            (var player, var side) = k_FENToCastling[c];

            // character was not '-'
            // not a guard clause because compiler uses HasValue check for casting Nullable<>
            if (player.HasValue && side.HasValue)
            {
                if (m_castling[player.Value].Contains(side.Value))
                {
                    throw new ArgumentException($"Duplicate character \'{c}\' in FEN file");
                }
                m_castling[player.Value].Add(side.Value);
            }
            else
            {
                if (castling.Length > 1)
                {
                    throw new ArgumentException($"Character \'{k_noCastling}\' must exclusively represent null castling rights in FEN file");
                }
                break;
            }
        }
    }

    /// <summary>
    /// Accessor for the castling privileges. Note: order matters
    /// </summary>
    /// <returns>The castling privileges in .FEN format</returns>
    public string GetCastling()
    {
        var castlingString = new StringBuilder(k_FENToCastling.Keys.Count - 1);
        foreach (((var player, var castling), var c) in k_castlingToFEN)
        {
            if (m_castling[player].Contains(castling))
            {
                castlingString.Append(c);
            }
        }
        if (castlingString.Length == 0)
        {
            return k_noCastling.ToString();
        }
        return castlingString.ToString();
    }

    /// <summary>
    /// Remove partial castling rights from a player
    /// </summary>
    /// <param name="player">The player who loses a castling right</param>
    /// <param name="side">The side on which the player loses castling rights</param>
    public void RemoveCastling(Player player, Castling side)
    {
        m_castling[player].Remove(side);
    }

    /// <summary>
    /// Check if a player may castle to a side in the current board state (FEN). Does not check for the legality of the move itself.
    /// </summary>
    /// <param name="player">The player that wishes to castle</param>
    /// <param name="side">The side on which the player wishes to castle</param>
    public bool CanCastleFromFEN(Player player, Castling side)
    {
        List<Castling>? castling;
        try
        {
            m_castling.TryGetValue(player, out castling);
            return (castling is not null && castling.Contains(side));
        }
        catch (ArgumentNullException)
        {
            return false;
        }
    }

    /// <summary>
    /// Handle the capture that has occurred on a specific square
    /// </summary>
    /// <param name="attacker">The piece that initiated the capture</param>
    /// <param name="coordinate">The square on which the capture takes place</param>
    public void Capture(Piece attacker, (int, int) coordinate)
    {
        GetRuleset().Capture(attacker, this, GameHelpers.GetSquareFromCoordinate(coordinate));
        if (m_game is not null)
        {
            m_game.ClearVisitedPositions();
        }
        m_halfmoveClock = 0;
    }

    /// <summary>
    /// Handle a pawn's request to promote
    /// </summary>
    /// <param name="pawn">The pawn that wishes to promote</param>
    public void RequestPromotion(Pawn pawn)
    {
        try
        {
            pawn.Destroy();
            var promotionPiece = (pawn.GetPlayer() == Player.White) ? Char.ToUpper(RequestPromotionPiece()) : Char.ToLower(RequestPromotionPiece());
            CreatePiece(promotionPiece, pawn.GetCoordinates());
        }
        catch (ArgumentException)
        {
            // Caught if the received promotion piece is invalid or if the pawn is out of bounds
            Console.WriteLine("Error! Promotion request failed.");
        }
    }

    /// <summary>
    /// Request from the user which piece to promote a pawn to
    /// </summary>
    /// <returns>The character corresponding to the piece to which the pawn will promote</returns>
    public char RequestPromotionPiece()
    {
        // TODO: Ask the user for which piece to promote to
        return 'Q';
    }

    public override string ToString()
    {
        var output = "";

        for (var y = GameHelpers.k_boardHeight - 1; y >= 0; y--)
        {
            for (var x = 0; x < GameHelpers.k_boardWidth; x++)
            {
                var piece = GetPiece((x, y));
                if (piece is null)
                {
                    output += ".";
                }
                else
                {
                    output += piece.ToString();
                }
            }
            output += "\n";
        }

        return output;
    }
}