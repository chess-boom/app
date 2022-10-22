using System;
using System.IO;

namespace ChessBoom.GameBoard
{
    enum Variant {
        Standard,
        Chess960,
        Atomic,
        Horde
    }

    public enum Player {
        White,
        Black
    }
    public class Game {
        private Variant m_variant = Variant.Standard;
        private Board m_board;
        private Player m_playerToPlay {get; set;} = Player.White;
        private int m_halfmoveClock = 0;
        private int m_fullmoveCount = 0;

        public Game() {
            m_board = new Board();
            InitializeBoard(m_variant);

            System.Console.WriteLine(m_board.ToString());
        }

        /*public Game(Variant variant) {
            m_variant = variant;
            Game();
        }*/

        private void InitializeBoard(Variant variant) {
            string fen = "";

            // Note: Standard and Atomic use the default board. Chess960 and Horde use different initial configurations
            switch (variant) {
                case Variant.Chess960:
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

        private void CreateBoardFromFEN(string fen) {
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
            for (int row = 7; row >= 0; row--) {
                int col = 0;

                foreach (char piece in pieceSplit[row]) {
                    if (Char.IsDigit(piece)) {
                        col += (int)Char.GetNumericValue(piece);
                    } else {
                        try {
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
            if (fenSplit[1].Length != 1) {
                throw new ArgumentException($"Player \"{fenSplit[1]}\" does not denote a single player.");
            }
            try {
                SetPlayerToPlay(fenSplit[1][0]);
            }
            catch (ArgumentException) {
                SetPlayerToPlay('w');
            }

            // Set castling availability

            // Set en passant capability

            // Set halfmove clock
            try {
                m_halfmoveClock = int.Parse(fenSplit[4]);
            }
            catch (FormatException) {
                m_halfmoveClock = 0;
            }

            // Set fullmove number
            try {
                m_fullmoveCount = int.Parse(fenSplit[5]);
            }
            catch (FormatException) {
                m_fullmoveCount = 0;
            }
        }

        private void SetPlayerToPlay(char player) {
            switch (player) {
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
    }
}