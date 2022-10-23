using System;
using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    public class Board {
        private List<Piece> m_pieces;

        public Board() {
            m_pieces = new List<Piece>();
        }

        public Piece? GetPiece(int row, int col) {
            foreach (Piece piece in m_pieces) {
                if ((row, col) == piece.GetCoordinates()) {
                    return piece;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a piece
        /// </summary>
        /// <throws>ArgumentException if invalid pieceType or invalid coordinates</throws>
        public void CreatePiece(char pieceType, int row, int col) {
            if (row < 0 || row >= GameHelpers.k_BoardHeight || col < 0 || col >= GameHelpers.k_BoardWidth) {
                throw new ArgumentException($"Coordinate ({col}, {row}) is an invalid coordinate (x, y).");
            }

            Piece piece;
            switch(pieceType) {
                case 'K':
                    piece = new King(Player.White, row, col);
                    break;
                case 'k':
                    piece = new King(Player.Black, row, col);
                    break;
                case 'Q':
                    piece = new Queen(Player.White, row, col);
                    break;
                case 'q':
                    piece = new Queen(Player.Black, row, col);
                    break;
                case 'B':
                    piece = new Bishop(Player.White, row, col);
                    break;
                case 'b':
                    piece = new Bishop(Player.Black, row, col);
                    break;
                case 'N':
                    piece = new Knight(Player.White, row, col);
                    break;
                case 'n':
                    piece = new Knight(Player.Black, row, col);
                    break;
                case 'R':
                    piece = new Rook(Player.White, row, col);
                    break;
                case 'r':
                    piece = new Rook(Player.Black, row, col);
                    break;
                case 'P':
                    piece = new Pawn(Player.White, row, col);
                    break;
                case 'p':
                    piece = new Pawn(Player.Black, row, col);
                    break;
                default:
                    throw new ArgumentException($"Error. {pieceType} is an invalid piece type.");
            }

            m_pieces.Add(piece);
        }

        public override string ToString() {
            string output = "";
            foreach (Piece piece in m_pieces) {
                output += piece;
            }
            return output;
        }
    }
}