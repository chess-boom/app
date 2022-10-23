using System;

namespace ChessBoom.GameBoard
{
    public static class GameHelpers {
        public static string[] k_BoardColumnNames = {"a", "b", "c", "d", "e", "f", "g", "h"};
        public static string[] k_BoardRowNames = {"1", "2", "3", "4", "5", "6", "7", "8"};

        public static int k_BoardWidth = 8;
        public static int k_BoardHeight = 8;

        public static string GetSquareFromCoordinate((int row, int col) coordinate) {
            if (coordinate.Item1 < 0 || coordinate.Item1 >= k_BoardHeight || coordinate.Item2 < 0 || coordinate.Item2 >= k_BoardHeight) {
                throw new ArgumentException($"Coordinate ({coordinate.Item1}, {coordinate.Item2}) is an invalid square");
            }

            return coordinate.Item1.ToString() + coordinate.Item2.ToString();
        }
        public static (int, int) GetCoordinateFromSquare(string square) {
            if (square.Length != 2) {
                throw new ArgumentException($"{square} is not properly formatted.");
            }

            // Get the column
            int column = -1;
            for (int index = 0; index < k_BoardColumnNames.Length; index++) {
                if (square[0].Equals(k_BoardColumnNames[index])) {
                    column = index;
                    break;
                }
            }
            if (column == -1) {
                throw new ArgumentException($"{square} does not have a proper column coordinate.");
            }

            // Get the row
            int row = -1;
            for (int index = 0; index < k_BoardRowNames.Length; index++) {
                if (square[1].Equals(k_BoardRowNames[index])) {
                    row = index;
                    break;
                }
            }
            if (row == -1) {
                throw new ArgumentException($"{square} does not have a proper row coordinate.");
            }

            return (row, column);
        }
    }
}