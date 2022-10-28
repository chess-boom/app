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

        public Piece(Board board, Player player, int row, int column)
        {
            m_board = board;
            m_owner = player;
            m_row = row;
            m_column = column;
        }

        public void Destroy()
        {
            m_board.m_pieces.Remove(this);
        }

        public (int, int) GetCoordinates()
        {
            return (m_row, m_column);
        }

        public abstract List<(int, int)> GetMovementSquares();

        public bool CanMoveToSquare(string squareName)
        {
            return GetMovementSquares().Contains(GameHelpers.GetCoordinateFromSquare(squareName));
        }

        public void MovePiece((int, int) coordinate)
        {
            if (GetMovementSquares().Contains(coordinate))
            {
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