using System;
using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    public abstract class Piece
    {
        protected Board m_board;
        protected Player m_owner;
        protected int m_row;
        protected int m_column;

        public Piece(Board board, Player player, (int, int) coordinate)
        {
            m_board = board;
            m_owner = player;
            m_row = coordinate.Item2;
            m_column = coordinate.Item1;
        }

        /// <summary>
        /// Effectively destroy this piece
        /// </summary>
        public virtual void Destroy()
        {
            m_board.m_pieces.Remove(this);
        }

        public (int, int) GetCoordinates()
        {
            return (m_column, m_row);
        }

        /// <summary>
        /// Get the list of squares that this piece can move to
        /// TODO: Convert to HashSet to improve performance
        /// </summary>
        /// <returns>The list of square coordinates that the piece can move to</returns>
        public abstract List<(int, int)> GetMovementSquares();

        public bool CanMoveToSquare(string squareName)
        {
            return GetMovementSquares().Contains(GameHelpers.GetCoordinateFromSquare(squareName));
        }

        /// <summary>
        /// Attempt to move the piece to a new square. Initiates a capture if required
        /// </summary>
        /// <param name="coordinate">The coordinate to which the piece will try to move</param>
        /// <exception cref="ArgumentException">Thrown the piece is unable to move to the specified coordinate</exception>
        public virtual void MovePiece((int, int) coordinate)
        {
            if (GetMovementSquares().Contains(coordinate))
            {
                if (m_board.m_enPassant != null)
                {
                    m_board.m_enPassant = null;
                }

                if (m_board.GetPiece(coordinate) != null)
                {
                    m_board.Capture(this, coordinate);
                }
                m_column = coordinate.Item1;
                m_row = coordinate.Item2;
            }
            else
            {
                throw new ArgumentException($"Error. Piece {this.ToString()} on {GameHelpers.GetSquareFromCoordinate(GetCoordinates())} is unable to move to {GameHelpers.GetSquareFromCoordinate(coordinate)}!");
            }
        }

        /// <summary>
        /// Forcibly move the piece to a specific square, regardless of if it would be normally allowed
        /// </summary>
        /// <param name="coordinate">The coordinate to which the piece will move</param>
        /// <exception cref="ArgumentException">The passed coordinate is out of bounds</exception>
        public void CommandMovePiece((int, int) coordinate)
        {
            if (!GameHelpers.IsOnBoard(coordinate))
            {
                throw new ArgumentException($"Coordinate {coordinate} is not on the board!");
            }
            if (m_board.m_enPassant != null)
            {
                m_board.m_enPassant = null;
            }

            if (m_board.GetPiece(coordinate) != null)
            {
                m_board.Capture(this, coordinate);
            }
            m_column = coordinate.Item1;
            m_row = coordinate.Item2;
        }

        public Board GetBoard()
        {
            return m_board;
        }

        public Player GetPlayer()
        {
            return m_owner;
        }
    }
}