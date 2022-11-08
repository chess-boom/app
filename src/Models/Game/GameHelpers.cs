using System;
using System.Collections.Generic;

namespace ChessBoom.Models.Game
{
    /// <summary>
    /// This helper class contains useful constants and functions for use in other gameplay-related classes
    /// </summary>
    public static class GameHelpers
    {
        /// <summary>
        /// The names of the columns of the chess board are here defined
        /// </summary>
        public static readonly string[] k_BoardColumnNames = { "a", "b", "c", "d", "e", "f", "g", "h" };
        /// <summary>
        /// The names of the rows of the chess board are here defined
        /// </summary>
        public static readonly string[] k_BoardRowNames = { "1", "2", "3", "4", "5", "6", "7", "8" };

        /// <summary>
        /// The width of the chess board is here defined
        /// </summary>
        public static readonly int k_BoardWidth = 8;
        /// <summary>
        /// The height of the chess board is here defined
        /// </summary>
        public static readonly int k_BoardHeight = 8;

        /// <summary>
        /// The corresponding square is retrieved from its board coordinates
        /// </summary>
        /// <param name="coordinate">The 2-tuple containing the row and column coordinates (0-7, 0-7)</param>
        /// <exception cref="ArgumentException">Thrown when the square coordinates are invalid</exception>
        /// <returns>The corresponding board square as a string</returns>
        public static string GetSquareFromCoordinate((int row, int col) coordinate)
        {
            if (coordinate.Item1 < 0 || coordinate.Item1 >= k_BoardHeight || coordinate.Item2 < 0 || coordinate.Item2 >= k_BoardHeight)
            {
                throw new ArgumentException($"Coordinate ({coordinate.Item1}, {coordinate.Item2}) is an invalid square");
            }

            return k_BoardColumnNames[coordinate.Item1] + k_BoardRowNames[coordinate.Item2];
        }

        /// <summary>
        /// The corresponding coordinates is retrieved from its board square
        /// </summary>
        /// <param name="square">The string denoting the board square (ex: "h5")</param>
        /// <exception cref="ArgumentException">Thrown when the board square is invalid</exception>
        /// <returns>The corresponding board coordinates as a 2-tuple (0-7, 0-7)</returns>
        public static (int, int) GetCoordinateFromSquare(string square)
        {
            if (square.Length != 2)
            {
                throw new ArgumentException($"{square} is not properly formatted.");
            }

            // Get the column
            int column = -1;
            for (int index = 0; index < k_BoardColumnNames.Length; index++)
            {
                if (square[0].ToString().Equals(k_BoardColumnNames[index]))
                {
                    column = index;
                    break;
                }
            }
            if (column == -1)
            {
                throw new ArgumentException($"{square} does not have a proper column coordinate.");
            }

            // Get the row
            int row = -1;
            for (int index = 0; index < k_BoardRowNames.Length; index++)
            {
                if (square[1].ToString().Equals(k_BoardRowNames[index]))
                {
                    row = index;
                    break;
                }
            }
            if (row == -1)
            {
                throw new ArgumentException($"{square} does not have a proper row coordinate.");
            }

            return (column, row);
        }

        /// <summary>
        /// Check for if a given coordinate exists on a board
        /// </summary>
        /// <param name="coordinate">The 2-tuple containing the row and column coordinates (0-7, 0-7)</param>
        /// <returns>Whether or not the coordinate exists on a board</returns>
        public static bool IsOnBoard((int, int) coordinate)
        {
            return (coordinate.Item1 >= 0 && coordinate.Item1 < k_BoardWidth
                && coordinate.Item2 >= 0 && coordinate.Item2 < k_BoardHeight);
        }

        /// <summary>
        /// Get the opposing player
        /// </summary>
        /// <param name="player">The player seeking their opponent</param>
        /// <returns>The opposing player</returns>
        public static Player GetOpponent(Player player)
        {
            return (player == Player.White ? Player.Black : Player.White);
        }

        /// <summary>
        /// Get all the player's pieces
        /// </summary>
        /// <param name="player">The player seeking their pieces</param>
        /// <param name="board">The board on which the pieces reside</param>
        /// <returns>The player's pieces</returns>
        public static List<Piece> GetPlayerPieces(Player player, Board board)
        {
            List<Piece> playerPieces = new List<Piece>();
            foreach (Piece piece in board.m_pieces)
            {
                if (piece.GetPlayer() == player)
                {
                    playerPieces.Add(piece);
                }
            }
            return playerPieces;
        }

        /// <summary>
        /// Check for if a given square is under attack by another player
        /// </summary>
        /// <param name="board">The board to examine</param>
        /// <param name="searchingPlayer">The player who might be able to see the passed coordinate</param>
        /// <param name="coordinate">The 2-tuple containing the row and column coordinates (0-7, 0-7)</param>
        /// <returns>Whether or not the coordinate is visible to the player</returns>
        public static bool IsSquareVisible(Board board, Player searchingPlayer, (int, int) coordinate)
        {
            foreach (Piece piece in GameHelpers.GetPlayerPieces(searchingPlayer, board))
            {
                try
                {
                    if (piece.CanMoveToSquare(GameHelpers.GetSquareFromCoordinate(coordinate)))
                    {
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Mathematically add two 2-dimensional integer vectors
        /// </summary>
        /// <param name="v1">A 2-dimensional integer vector</param>
        /// <param name="v2">A 2-dimensional integer vector</param>
        /// <returns>The sum of the two vectors</returns>
        public static (int, int) AddVector((int, int) v1, (int, int) v2)
        {
            (int, int) vector = v1;
            vector.Item1 += v2.Item1;
            vector.Item2 += v2.Item2;
            return vector;
        }

        /// <summary>
        /// Find all possible movement squares by repeatedly adding a movement vector to a starting position and add them to a List
        /// </summary>
        /// <param name="movementSquares">The reference of the list of squares that is added to. Squares added in order from closest to furthest from origin</param>
        /// <param name="board">The board on which the movement is made</param>
        /// <param name="player">The player considered an "ally"</param>
        /// <param name="startingPosition">The position (not counted) at which the movement begins</param>
        /// <param name="movementVector">The vector that is repeatedly added to the current position</param>
        public static void GetVectorMovementSquares(ref List<(int, int)> movementSquares, Board board, Player player, (int, int) startingPosition, (int, int) movementVector)
        {
            if (!IsOnBoard(startingPosition))
            {
                return;
            }

            (int, int) position = startingPosition;
            bool movementFlag = true;
            do
            {
                position = AddVector(position, movementVector);

                // Handle board boundaries
                if (!GameHelpers.IsOnBoard(position))
                {
                    movementFlag = false;
                    break;
                }

                Piece? occupant = board.GetPiece(position);
                // Handle running into a piece
                if (occupant != null)
                {
                    movementFlag = false;
                    // Allied pieces can not be captured
                    if (occupant.GetPlayer() == player)
                    {
                        break;
                    }
                }

                movementSquares.Add((position.Item1, position.Item2));
            }
            while (movementFlag);
        }
    }
}