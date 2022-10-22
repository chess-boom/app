using System;

namespace ChessBoom.Game
{
    public static class GameHelpers {
        public static string GetSquareNameFromCoordinates(int row, int col) {
            if (row < 0 || row >= Board.k_BoardRowNames.Length) {
                throw new ArgumentException($"{row} is outside of the number of board rows.");
            }
            if (col < 0 || col >= Board.k_BoardColumnNames.Length) {
                throw new ArgumentException($"{col} is outside of the number of board columns.");
            }

            return Board.k_BoardColumnNames[col] + Board.k_BoardRowNames[row];
        }

        public static (int, int) GetCoordinatesFromSquareName(string name) {
            if (name.Length != 2) {
                throw new ArgumentException($"{name} is not properly formatted.");
            }

            // Get the column
            int column = -1;
            for (int index = 0; index < Board.k_BoardColumnNames.Length; index++) {
                if (name[0].Equals(Board.k_BoardColumnNames[index])) {
                    column = index;
                    break;
                }
            }
            if (column == -1) {
                throw new ArgumentException($"{name} does not have a proper column coordinate.");
            }

            // Get the row
            int row = -1;
            for (int index = 0; index < Board.k_BoardRowNames.Length; index++) {
                if (name[1].Equals(Board.k_BoardRowNames[index])) {
                    row = index;
                    break;
                }
            }
            if (row == -1) {
                throw new ArgumentException($"{name} does not have a proper row coordinate.");
            }

            return (row, column);
        }
    }
}