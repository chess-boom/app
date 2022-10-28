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
        public Board m_board { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Game()
        {
            m_board = new Board();
            InitializeBoard(m_variant);

            m_ruleset = new Standard();

            System.Console.WriteLine(CreateFENFromBoard(m_board));
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

            m_board = CreateBoardFromFEN(fen);
        }

        public void Capture(Piece attacker, string square)
        {
            m_ruleset.Capture(attacker, m_board, square);
        }

        /// <summary>
        /// The board is created and populated from a .FEN file
        /// </summary>
        /// <param name="fen">The contents of the .FEN file</param>
        private Board CreateBoardFromFEN(string fen)
        {
            Board board = new Board();
            /* FEN files have 6 parts, delimited by ' ' characters:
            The first part is the piece placements, rows delimited by '/' characters starting on the top
            The second part denotes the next player to take their turn
            The third part denotes castling availability
            The fourth part denotes en passant availability
            The fifth part denotes the halfmove clock, useful for enforcing the fifty-move rule
            The sixth part denotes the fullmove number */
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
                            board.CreatePiece(piece, row, col);
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
            catch (ArgumentException)
            {
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
            string fen = "";

            // Retrieve the pieces
            for (int row = 0; row < GameHelpers.k_BoardHeight; row++)
            {
                int emptySquareCount = 0;
                for (int col = 0; col < GameHelpers.k_BoardWidth; col++)
                {
                    Piece? piece = board.GetPiece((row, col));
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
            fen += (board.m_playerToPlay == Player.White) ? "w" : "b";
            //
            fen += " ";
            // Retrieve castling availability
            fen += board.GetCastling();
            //
            fen += " ";
            // Retrieve en passant capability
            try
            {
                if (board.m_enPassant.HasValue)
                {
                    fen += GameHelpers.GetSquareFromCoordinate(board.m_enPassant.Value);
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
            fen += board.m_halfmoveClock.ToString();
            //
            fen += " ";
            // Retrieve fullmove number
            fen += board.m_fullmoveCount.ToString();

            return fen;
        }
    }
}