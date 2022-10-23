using System;
using System.Collections.Generic;
using System.IO;

namespace ChessBoom.GameBoard
{
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

    enum Castling
    {
        Kingside,
        Queenside
    }

    /// <summary>
    /// The Game class handles the creation and playing of a game of any chess variant
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The chosen variant for this game
        /// </summary>
        private Variant m_variant = Variant.Standard;
        /// <summary>
        /// The chosen ruleset for this game
        /// </summary>
        private Ruleset m_ruleset;
        /// <summary>
        /// The board created for this game
        /// </summary>
        private Board m_board;
        /// <summary>
        /// The next player to move
        /// </summary>
        private Player m_playerToPlay { get; set; } = Player.White;
        /// <summary>
        /// The ability for each player to castle
        /// </summary>
        private Dictionary<Player, List<Castling>> m_castling;
        /// <summary>
        /// The square on which en passant may be played (if any)
        /// </summary>
        private (int, int)? m_enPassant;
        /// <summary>
        /// The number of half-moves since the last capture or pawn advance
        /// </summary>
        private int m_halfmoveClock = 0;
        /// <summary>
        /// The number of full moves (starting at 1, incremented after Black moves)
        /// </summary>
        private int m_fullmoveCount = 0;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Game()
        {
            m_board = new Board();
            m_castling = new Dictionary<Player, List<Castling>>();
            InitializeBoard(m_variant);

            m_ruleset = new Standard();

            System.Console.WriteLine(CreateFENFromBoard());
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
        private void InitializeBoard(Variant variant)
        {
            string fen = "";

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

            CreateBoardFromFEN(fen);
        }

        /// <summary>
        /// The board is created and populated from a .FEN file
        /// </summary>
        /// <param name="fen">The contents of the .FEN file</param>
        private void CreateBoardFromFEN(string fen)
        {
            /* FEN files have 6 parts, delimited by ' ' characters:
            The first part is the piece placements, rows delimited by '/' characters starting on the top
            The second part denotes the next player to take their turn
            The third part denotes castling availability
            The fourth part denotes en passant availability
            The fifth part denotes the halfmove clock, useful for enforcing the fifty-move rule
            The sixth part denotes the fullmove number
            */
            string[] fenSplit = fen.Split(' ');

            // Create the pieces
            string[] pieceSplit = fenSplit[0].Split('/');
            for (int row = 0; row < GameHelpers.k_BoardHeight; row++)
            {
                int col = 0;

                foreach (char piece in pieceSplit[row])
                {
                    if (Char.IsDigit(piece))
                    {
                        col += (int)Char.GetNumericValue(piece);
                    }
                    else
                    {
                        try
                        {
                            m_board.CreatePiece(piece, row, col);
                        }
                        catch (ArgumentException)
                        {

                        }
                        col++;
                    }
                }
            }

            // Set the next player to play
            if (fenSplit[1].Length != 1)
            {
                throw new ArgumentException($"Player \"{fenSplit[1]}\" does not denote a single player.");
            }
            try
            {
                SetPlayerToPlay(fenSplit[1][0]);
            }
            catch (ArgumentException)
            {
                SetPlayerToPlay('w');
            }

            // Set castling availability
            try
            {
                SetCastling(fenSplit[2]);
            }
            catch (ArgumentException)
            {
                SetCastling("-");
            }

            // Set en passant capability
            if (fenSplit[3] != "-")
            {
                try
                {
                    m_enPassant = GameHelpers.GetCoordinateFromSquare(fenSplit[3]);
                }
                catch (ArgumentException)
                {
                    m_enPassant = null;
                }
            }
            else
            {
                m_enPassant = null;
            }

            // Set halfmove clock
            try
            {
                m_halfmoveClock = int.Parse(fenSplit[4]);
            }
            catch (FormatException)
            {
                m_halfmoveClock = 0;
            }

            // Set fullmove number
            try
            {
                m_fullmoveCount = int.Parse(fenSplit[5]);
            }
            catch (FormatException)
            {
                m_fullmoveCount = 0;
            }
        }

        /// <summary>
        /// Mutator for the next player to play
        /// </summary>
        /// <param name="player">The .FEN notation of the player (w, b)</param>
        /// <exception cref="ArgumentException">Thrown when the passed player is invalid</exception>
        private void SetPlayerToPlay(char player)
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
        private void SetCastling(string castling)
        {
            List<Castling> whiteCastling = new List<Castling>();
            List<Castling> blackCastling = new List<Castling>();
            foreach (char c in castling)
            {
                switch (c)
                {
                    case 'K':
                        whiteCastling.Add(Castling.Kingside);
                        break;
                    case 'k':
                        blackCastling.Add(Castling.Kingside);
                        break;
                    case 'Q':
                        whiteCastling.Add(Castling.Queenside);
                        break;
                    case 'q':
                        blackCastling.Add(Castling.Queenside);
                        break;
                    case '-':
                        break;
                    default:
                        throw new ArgumentException($"Invalid character {c} in FEN file.");
                }
            }

            m_castling.Add(Player.White, whiteCastling);
            m_castling.Add(Player.Black, blackCastling);
        }

        /// <summary>
        /// Accessor for the castling privileges
        /// </summary>
        /// <returns>The castling privileges in .FEN format</returns>
        private string GetCastling()
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
            return castling;
        }

        /// <summary>
        /// Retrieve the board state as the contents of a .FEN file
        /// </summary>
        /// <returns>The board state as the contents of a .FEN file</returns>
        private string CreateFENFromBoard()
        {
            string fen = "";

            // Retrieve the pieces
            for (int row = 0; row < GameHelpers.k_BoardHeight; row++)
            {
                int emptySquareCount = 0;
                for (int col = 0; col < GameHelpers.k_BoardWidth; col++)
                {
                    Piece? piece = m_board.GetPiece((row, col));
                    if (piece == null)
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

                if (row != GameHelpers.k_BoardHeight - 1)
                {
                    fen += "/";
                }
            }

            //
            fen += " ";
            // Retrieve the next player
            fen += (m_playerToPlay == Player.White) ? "w" : "b";
            //
            fen += " ";
            // Retrieve castling availability
            fen += GetCastling();
            //
            fen += " ";
            // Retrieve en passant capability
            try
            {
                if (m_enPassant.HasValue)
                {
                    fen += GameHelpers.GetSquareFromCoordinate(m_enPassant.Value);
                }
                else
                {
                    fen += "-";
                }
            }
            catch (ArgumentException)
            {
                fen += "-";
            }
            //
            fen += " ";
            // Retrieve halfmove clock
            fen += m_halfmoveClock.ToString();
            //
            fen += " ";
            // Retrieve fullmove number
            fen += m_fullmoveCount.ToString();

            return fen;
        }
    }
}