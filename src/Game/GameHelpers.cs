using System;

namespace ChessBoom.GameBoard
{
    /// <summary>
    /// This helper class contains useful constants and functions for use in other gameplay-related classes
    /// </summary>
    public static class GameHelpers {
        /// <summary>
        /// The names of the columns of the chess board are here defined
        /// </summary>
        public static string[] k_BoardColumnNames = {"a", "b", "c", "d", "e", "f", "g", "h"};
        /// <summary>
        /// The names of the rows of the chess board are here defined
        /// </summary>
        public static string[] k_BoardRowNames = {"1", "2", "3", "4", "5", "6", "7", "8"};

        /// <summary>
        /// The width of the chess board is here defined
        /// </summary>
        public static int k_BoardWidth = 8;
        /// <summary>
        /// The height of the chess board is here defined
        /// </summary>
        public static int k_BoardHeight = 8;

        /// <summary>
        /// The corresponding square is retrieved from its board coordinates
        /// </summary>
        /// <param name="coordinate">The 2-tuple containing the row and column coordinates (0-7, 0-7)</param>
        /// <exception cref="ArgumentException">Thrown when the square coordinates are invalid</exception>
        /// <returns>The corresponding board square as a string</returns>
        public static string GetSquareFromCoordinate((int row, int col) coordinate) {
            if (coordinate.Item1 < 0 || coordinate.Item1 >= k_BoardHeight || coordinate.Item2 < 0 || coordinate.Item2 >= k_BoardHeight) {
                throw new ArgumentException($"Coordinate ({coordinate.Item1}, {coordinate.Item2}) is an invalid square");
            }

            return coordinate.Item1.ToString() + coordinate.Item2.ToString();
        }

        /// <summary>
        /// The corresponding coordinates is retrieved from its board square
        /// </summary>
        /// <param name="square">The string denoting the board square (ex: "h5")</param>
        /// <exception cref="ArgumentException">Thrown when the board square is invalid</exception>
        /// <returns>The corresponding board coordinates as a 2-tuple (0-7, 0-7)</returns>
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