using System;
using System.Linq;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Skia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ChessBoom.Models.Game;
using ChessBoom.ViewModels;
using ReactiveUI;
using SkiaSharp;
using Svg.Skia;

namespace ChessBoom.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    /// <summary>
    /// Defines attributes related to rendered Tiles
    /// </summary>
    private abstract class Tile
    {
        internal static int Width => 100;
        internal static int Height => 100;

        internal static readonly Color k_white = Color.FromRgb(243, 219, 180);
        internal static readonly Color k_black = Color.FromRgb(179, 140, 99);
    }

    /// <summary>
    /// Defines attributes related to rendered Pieces
    /// </summary>
    private abstract class Piece
    {
        internal const string k_white = "Assets/Pieces/{0}.svg";
        internal const string k_black = "Assets/Pieces/{0}_.svg";
    }

    /// <summary>
    /// Load BoardView into the ViewModel
    /// </summary>
    public BoardView()
    {
        this.WhenActivated(_ =>
        {
            DrawChessBoard();
            DrawPieces();
        });
        AvaloniaXamlLoader.Load(this);
        // WARN : VS Code Complains: `The name 'ChessBoard' does not exist in the current context [ChessBoom]csharp(CS0103)`
        // ChessBoard is defined at build time in Avalonia.NameGenerator/Avalonia.NameGenerator.AvaloniaNameSourceGenerator/BoardView.g.cs
        ChessBoard = this.Find<Grid>("ChessBoard");
    }

    /// <summary>
    /// Initialize the ChessBoard Control with tiles (Rectangles) and Piece placeholders (SKBitmapControl)
    /// </summary>
    private void DrawChessBoard()
    {
        for (var row = GameHelpers.k_boardHeight - 1; row >= 0; row--)
        {
            ChessBoard.RowDefinitions.Add(new RowDefinition());
            for (var col = 0; col < GameHelpers.k_boardWidth; col++)
            {
                var square = GameHelpers.GetSquareFromCoordinate((GameHelpers.k_boardHeight - (row + 1), col));

                var tile = new Rectangle
                {
                    Width = Tile.Width,
                    Height = Tile.Height,
                    Name = square,
                    StrokeThickness = 0,
                    Fill = (row + col) % 2 == 0 ? new SolidColorBrush(Tile.k_white) : new SolidColorBrush(Tile.k_black)
                };

                Grid.SetRow(tile, row);
                Grid.SetColumn(tile, col);
                ChessBoard.Children.Add(tile);

                var bitmap = new SKBitmapControl
                {
                    Width = Tile.Width,
                    Height = Tile.Height,
                    Name = square,
                    ZIndex = 1
                };

                // each square gets an SKBitmapControl to be able to render a piece svg
                Grid.SetRow(bitmap, row);
                Grid.SetColumn(bitmap, col);
                ChessBoard.Children.Add(bitmap);
            }
            ChessBoard.ColumnDefinitions.Add(new ColumnDefinition());
        }
    }

    /// <summary>
    /// Draw the Game's pieces to the ChessBoard Grid Control
    /// </summary>
    private void DrawPieces()
    {
        if (ViewModel == null) return;
        foreach (var piece in ViewModel.Game.m_board.m_pieces)
        {
            var piecePath = piece.GetPlayer() switch
            {
                Player.White => string.Format(Piece.k_white, piece),
                Player.Black => string.Format(Piece.k_black, piece),
                _ => throw new ArgumentOutOfRangeException()
            };

            var (col, row) = piece.GetCoordinates();
            var square = GameHelpers.GetSquareFromCoordinate((row, col));
            var bitmap = ChessBoard.Children.OfType<SKBitmapControl>().FirstOrDefault(x => x.Name == square);

            if (bitmap == null) throw new ArgumentNullException();

            using (var svg = new SKSvg())
            {
                bitmap.Bitmap = new SKBitmap(Tile.Width, Tile.Height);
                svg.Load(piecePath);
                if (svg.Picture == null) throw new ArgumentNullException();
                var canvas = new SKCanvas(bitmap.Bitmap);
                var matrix = SKMatrix.CreateScale(
                    Tile.Width / svg.Picture.CullRect.Width,
                    Tile.Height / svg.Picture.CullRect.Height
                );
                canvas.DrawPicture(svg.Picture, ref matrix);
            }

            bitmap.InvalidateVisual();
        }
    }
}