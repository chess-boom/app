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
using Avalonia.Input;
using Avalonia.Layout;

namespace ChessBoom.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    SKBitmapControl sourcePiece;
    Control destinationTile;

    /// <summary>
    /// Defines attributes related to rendered Tiles
    /// </summary>
    private abstract class Tile
    {
        internal static int Width => 50;
        internal static int Height => 50;

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
        for(int i = 0; i < 8; i++)
        {
            ChessBoard.RowDefinitions.Add(new RowDefinition());
            ChessBoard.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for (var row = 0; row < GameHelpers.k_boardHeight; row++)
        {
            for (var col = 0; col < GameHelpers.k_boardWidth; col++)
            {
                var square = GameHelpers.GetSquareFromCoordinate((col, GameHelpers.k_boardHeight - (row + 1)));

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
        }

        // add the letters and numbers to the board
        ChessBoard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(Tile.Height) });
        ChessBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Tile.Width) });
        for (int i = 0; i < 8; i++)
        {
            var letter = new TextBlock { Text = ((char)('A' + i)).ToString(), FontWeight = FontWeight.Bold, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            ChessBoard.Children.Add(letter);
            Grid.SetRow(letter, 8);
            Grid.SetColumn(letter, i);
            var number = new TextBlock { Text = (8 - i).ToString(),  FontWeight = FontWeight.Bold,TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            ChessBoard.Children.Add(number);
            Grid.SetRow(number, i);
            Grid.SetColumn(number, 8);
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
            var square = GameHelpers.GetSquareFromCoordinate((col, row));
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
            Console.WriteLine(bitmap);
        }
    }
    private void ChessBoard_MouseLeftButtonDown(object sender, PointerPressedEventArgs e)
    {
        if (ViewModel == null) return;
        if (ViewModel.FirstClick)
        {
            sourcePiece = e.Source as SKBitmapControl;
            if (sourcePiece == null) return;
        }
        else
        {
            destinationTile = e.Source switch
            {
                Rectangle tile => (Rectangle)tile,
                SKBitmapControl tile => (SKBitmapControl)tile
            };
            var piece = ViewModel.Game.m_board.GetPiece(sourceCoordinates);
            if (piece == null) return;
            try
            {
                ViewModel.Game.MakeMove(piece, destinationTile.Name);
                SKBitmapControl destinationBitmap = ChessBoard.Children.OfType<SKBitmapControl>().FirstOrDefault(x => x.Name == destinationTile.Name);
                destinationBitmap.Bitmap = sourcePiece.Bitmap;
                sourcePiece.Bitmap = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        ViewModel.FirstClick = !ViewModel.FirstClick;
    }
}