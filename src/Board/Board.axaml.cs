namespace ChessBoom.Board
{
    public class Board {
        private static string[] k_BoardColumnNames = {"a", "b", "c", "d", "e", "f", "g", "h"};
        private static string[] k_BoardRowNames = {"1", "2", "3", "4", "5", "6", "7", "8"};

        private Square[,] m_board;

        public Board() {
            m_board = new Square[k_BoardRowNames.Length, k_BoardColumnNames.Length];
            for (int rowIndex = 0; rowIndex < k_BoardRowNames.Length; rowIndex++) {
                for (int colIndex = 0; colIndex < k_BoardColumnNames.Length; colIndex++) {
                    m_board[rowIndex, colIndex] = new Square(k_BoardColumnNames[colIndex] + k_BoardRowNames[rowIndex]);
                }
            }
        }
    }

    public class Square {
        private string m_name {get;}
        private Piece? m_occupant {get; set;}

        public Square(string name) {
            m_name = name;
            m_occupant = null;
        }
    }
}