using System;
using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    public class Board {
        private static string[] k_BoardColumnNames = {"a", "b", "c", "d", "e", "f", "g", "h"};
        private static string[] k_BoardRowNames = {"1", "2", "3", "4", "5", "6", "7", "8"};

        //private Square[,] m_board;
        private List<Piece> m_pieces;

        public Board() {

            m_pieces = new List<Piece>();

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
        public void CreatePiece(char pieceType, int row, int col) {

        }
    }

    /// May want to delete this class.
    public class Square {
        private string m_name {get;}
        private Piece? m_occupant {get; set;}

        public Square(int row, int col) {
            try {
                //m_name = GameHelpers.GetSquareNameFromCoordinates(row, col);
                m_name = ""; // to avoid getting warning
            }
            catch (ArgumentException) {
                m_name = "";
            }
            m_occupant = null;
        }
    }
}