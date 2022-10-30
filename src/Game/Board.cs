using System;
using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    /// <summary>
    /// The Board class contains the chess pieces and is the medium through which a Game is played
    /// </summary>
    public class Board
    {
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
        /// Retrieves a piece from its coordinates
        /// </summary>
        /// <param name="coordinate">The 2-tuple containing the row and column coordinates (0-7, 0-7)</param>
        /// <returns>The piece found on the passed square. If none, returns null</returns>
        public Piece? GetPiece((int, int) coordinate)
        {
            foreach (Piece piece in m_pieces)
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
        /// <throws>ArgumentException if invalid pieceType or invalid coordinates</throws>
        public void CreatePiece(char pieceType, (int, int) coordinate)
        {
            if (coordinate.Item1 < 0 || coordinate.Item1 >= GameHelpers.k_BoardWidth || coordinate.Item2 < 0 || coordinate.Item2 >= GameHelpers.k_BoardHeight)
            {
                throw new ArgumentException($"Coordinate ({coordinate.Item1}, {coordinate.Item2}) is an invalid coordinate (x, y).");
            }

            Piece piece;
            switch (pieceType)
            {
                case 'K':
                    piece = new King(this, Player.White, coordinate);
                    break;
                case 'k':
                    piece = new King(this, Player.Black, coordinate);
                    break;
                case 'Q':
                    piece = new Queen(this, Player.White, coordinate);
                    break;
                case 'q':
                    piece = new Queen(this, Player.Black, coordinate);
                    break;
                case 'B':
                    piece = new Bishop(this, Player.White, coordinate);
                    break;
                case 'b':
                    piece = new Bishop(this, Player.Black, coordinate);
                    break;
                case 'N':
                    piece = new Knight(this, Player.White, coordinate);
                    break;
                case 'n':
                    piece = new Knight(this, Player.Black, coordinate);
                    break;
                case 'R':
                    piece = new Rook(this, Player.White, coordinate);
                    break;
                case 'r':
                    piece = new Rook(this, Player.Black, coordinate);
                    break;
                case 'P':
                    piece = new Pawn(this, Player.White, coordinate);
                    break;
                case 'p':
                    piece = new Pawn(this, Player.Black, coordinate);
                    break;
                default:
                    throw new ArgumentException($"Error. {pieceType} is an invalid piece type.");
            }

            m_pieces.Add(piece);
        }

        /// <summary>
        /// Moves a piece from one square to another
        /// </summary>
        /// <param name="start">The name of the square on which the moving piece resides</param>
        /// <param name="destination">The name of the square to which the piece will move</param>
        /// <exception cref="ArgumentException">Thrown if the specified starting square is invalid/exception>
        /// <exception cref="ArgumentException">Thrown if the specified starting square contains no piece</exception>
        /// <exception cref="ArgumentException">Thrown if the found piece is unable to move to the specified coordinate</exception>
        public void MovePiece(string start, string destination)
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
            if (piece == null)
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
        }

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
        /// Mutator for castling privileges
        /// </summary>
        /// <param name="castling">The .FEN notation of the castling privileges (ex: "KQkq")</param>
        /// <exception cref="ArgumentException">Thrown when the castling privileges contains an invalid character</exception>
        public void SetCastling(string castling)
        {
            if (castling.Length < 1)
            {
                throw new ArgumentException($"FEN file must include castling rights.");
            }

            List<Castling> whiteCastling = new List<Castling>();
            List<Castling> blackCastling = new List<Castling>();
            foreach (char c in castling)
            {
                switch (c)
                {
                    case 'K':
                        if (whiteCastling.Contains(Castling.Kingside))
                        {
                            throw new ArgumentException($"Duplicate character \'{c}\' in FEN file.");
                        }
                        whiteCastling.Add(Castling.Kingside);
                        break;
                    case 'k':
                        if (blackCastling.Contains(Castling.Kingside))
                        {
                            throw new ArgumentException($"Duplicate character \'{c}\' in FEN file.");
                        }
                        blackCastling.Add(Castling.Kingside);
                        break;
                    case 'Q':
                        if (whiteCastling.Contains(Castling.Queenside))
                        {
                            throw new ArgumentException($"Duplicate character \'{c}\' in FEN file.");
                        }
                        whiteCastling.Add(Castling.Queenside);
                        break;
                    case 'q':
                        if (blackCastling.Contains(Castling.Queenside))
                        {
                            throw new ArgumentException($"Duplicate character \'{c}\' in FEN file.");
                        }
                        blackCastling.Add(Castling.Queenside);
                        break;
                    case '-':
                        if (whiteCastling.Count > 0 || blackCastling.Count > 0)
                        {
                            throw new ArgumentException($"Character \'-\' must represent null castling rights in FEN file.");
                        }
                        if (castling != "-")
                        {
                            throw new ArgumentException($"No castling rights must be represented by a single \'-\'.");
                        }
                        break;
                    default:
                        throw new ArgumentException($"Invalid character \'{c}\' in FEN file.");
                }
            }

            m_castling = new Dictionary<Player, List<Castling>>();
            m_castling.Add(Player.White, whiteCastling);
            m_castling.Add(Player.Black, blackCastling);
        }

        /// <summary>
        /// Accessor for the castling privileges
        /// </summary>
        /// <returns>The castling privileges in .FEN format</returns>
        public string GetCastling()
        {
            string castling = "";
            if (m_castling[Player.White].Contains(Castling.Kingside))
            {
                castling += "K";
            }
            if (m_castling[Player.White].Contains(Castling.Queenside))
            {
                castling += "Q";
            }
            if (m_castling[Player.Black].Contains(Castling.Kingside))
            {
                castling += "k";
            }
            if (m_castling[Player.Black].Contains(Castling.Queenside))
            {
                castling += "q";
            }
            if (castling == "")
            {
                castling = "-";
            }
            return castling;
        }

        /// <summary>
        /// Handle the capture that has occurred on a specific square
        /// </summary>
        /// <param name="attacker">The piece that initiated the capture</param>
        /// <param name="coordinate">The square on which the capture takes place</param>
        public void Capture(Piece attacker, (int, int) coordinate)
        {
            if (m_game != null)
            {
                m_game.Capture(attacker, GameHelpers.GetSquareFromCoordinate(coordinate));
            }
        }

        public override string ToString()
        {
            string output = "";
            foreach (Piece piece in m_pieces)
            {
                output += piece;
            }
            return output;
        }
    }
}