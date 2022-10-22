using System;
using System.Collections.Generic;

namespace ChessBoom.Game
{
    public class Board {
        private static string[] k_BoardColumnNames = {"a", "b", "c", "d", "e", "f", "g", "h"};
        private static string[] k_BoardRowNames = {"1", "2", "3", "4", "5", "6", "7", "8"};

        //private Square[,] m_board;
        private List<Piece> m_pieces;
        private int playerToPlay {get; set;} = 0;

        public Board() {

            /*
            m_board = new Square[k_BoardRowNames.Length, k_BoardColumnNames.Length];
            for (int rowIndex = 0; rowIndex < k_BoardRowNames.Length; rowIndex++) {
                for (int colIndex = 0; colIndex < k_BoardColumnNames.Length; colIndex++) {
                    m_board[rowIndex, colIndex] = new Square(rowIndex, colIndex);
                }
            }
            */
        }

        /// <summary>
        /// Creates a piece
        /// </summary>
        /// <throws>ArgumentException if invalid pieceType or invalid coordinates</throws>
        private void CreatePiece(char pieceType, int row, int col) {

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
            string[] pieceSplit = fenSplit[0].Split('/');

            // Create the pieces
            for (int row = 7; row >= 0; row--) {
                int col = 0;

                foreach (char piece in pieceSplit[row]) {
                    if (Char.IsDigit(piece)) {
                        col += Char.GetNumericValue(piece);
                    } else {
                        try {
                            CreatePiece(piece, row, col);
                        }
                        catch (ArgumentException e)
                        {

                        }
                        col++;
                    }
                }
            }

            //
        }
    }

    /// May want to delete this class.
    public class Square {
        private string m_name {get;}
        private Piece? m_occupant {get; set;}

        public Square(int row, int col) {
            try {
                m_name = GameHelpers.GetSquareNameFromCoordinates(row, col);
            }
            catch (ArgumentException e) {
                m_name = null;
            }
            m_occupant = null;
        }
    }
}