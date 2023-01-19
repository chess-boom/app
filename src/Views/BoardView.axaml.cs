using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ChessBoom.Models.Game.Pieces;
using ChessBoom.Models.Game;
using ChessBoom.ViewModels;
using ReactiveUI;
using SkiaSharp;
using Svg.Skia;
using System;
using Avalonia.Controls.Skia;

namespace ChessBoom.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    private abstract class Tile
    {
        internal static int Width => 50;
        internal static int Height => 50;

        internal static readonly Color k_white = Color.FromRgb(243, 219, 180);
        internal static readonly Color k_black = Color.FromRgb(179, 140, 99);
    }

    private abstract class Piece
    {
        internal const string k_white = "Assets/Pieces/{0}.svg";
        internal const string k_black = "Assets/Pieces/{0}_.svg";
    }

    public BoardView()
    {
        this.WhenActivated(_ =>
        {
            DrawChessBoard();
            DrawPieces();
        });
        AvaloniaXamlLoader.Load(this);
        ChessBoard = this.Find<Grid>("ChessBoard");
    }

    private void DrawChessBoard()
    {
        for (var i = 0; i < GameHelpers.k_boardWidth; i++)
        {
            ChessBoard.ColumnDefinitions.Add(new ColumnDefinition());
            for (var j = 0; j < GameHelpers.k_boardHeight; j++)
            {
                ChessBoard.RowDefinitions.Add(new RowDefinition());
                var tile = new Rectangle
                {
                    Width = Tile.Width,
                    Height = Tile.Height,
                    Name = GameHelpers.GetSquareFromCoordinate((i, j)),
                    StrokeThickness = 0,
                    Fill = (i + j) % 2 == 0 ? new SolidColorBrush(Tile.k_white) : new SolidColorBrush(Tile.k_black)
                };

                Grid.SetRow(tile, i);
                Grid.SetColumn(tile, j);
                ChessBoard.Children.Add(tile);

                var bitmap = new SKBitmapControl
                {
                    Width = Tile.Width,
                    Height = Tile.Height,
                    Name = GameHelpers.GetSquareFromCoordinate((i, j)),
                    ZIndex = 1
                };
                Grid.SetRow(bitmap, i);
                Grid.SetColumn(bitmap, j);
                ChessBoard.Children.Add(bitmap);
            }
        }
    }

    private void DrawPieces()
    {
        // In Progress
        var piece = new Pawn(ViewModel!.game.m_board, Player.Black, (0, 0));
        var piecePath = piece.GetPlayer() switch
        {
            Player.White => string.Format(Piece.k_white, piece),
            Player.Black => string.Format(Piece.k_black, piece),
            _ => throw new ArgumentOutOfRangeException()
        };
        string square = GameHelpers.GetSquareFromCoordinate(piece.GetCoordinates());
        SKBitmapControl bitmap = ChessBoard.Find<SKBitmapControl>(square);
        SKCanvas canvas = new SKCanvas(bitmap.Bitmap);
        var svg = new SKSvg();
        svg.Load(piecePath);
        canvas.DrawPicture(svg.Picture);

        // Grid.SetRow(canvas, 0);
        // Grid.SetColumn(canvas, 0);

        // foreach (var piece in ViewModel.game.m_board.m_pieces)
        // {
        //     var tile = this.Find<Rectangle>(GameHelpers.GetSquareFromCoordinate(piece.GetCoordinates()));
        //     var
        // }
    }
}