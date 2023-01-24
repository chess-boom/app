using System;
using System.Linq;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Skia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ChessBoom.Models.Game;
using ChessBoom.ViewModels;
using ReactiveUI;
using SkiaSharp;
using Svg.Skia;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace ChessBoom.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    private SKBitmapControl? _sourcePiece;
    private (int, int) _sourceCoordinates;

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
        ChessBoard = this.Find<Grid>("ChessBoard");
    }

    /// <summary>
    /// Initialize the ChessBoard Control with tiles (Rectangles) and Piece placeholders (SKBitmapControl)
    /// </summary>
    private void DrawChessBoard()
    {
        for (int i = 0; i < 8; i++)
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
        for (int i = 0; i < GameHelpers.k_boardWidth; i++)
        {
            var letter = new TextBlock
            {
                Text = ((char)('A' + i)).ToString(),
                FontWeight = FontWeight.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            ChessBoard.Children.Add(letter);
            Grid.SetRow(letter, GameHelpers.k_boardHeight);
            Grid.SetColumn(letter, i);
        }

        for (int i = 0; i < GameHelpers.k_boardHeight; i++)
        {
            var number = new TextBlock
            {
                Text = (GameHelpers.k_boardWidth - i).ToString(),
                FontWeight = FontWeight.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            ChessBoard.Children.Add(number);
            Grid.SetRow(number, i);
            Grid.SetColumn(number, GameHelpers.k_boardWidth);
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
        }
    }

    // ReSharper disable once UnusedParameter.Local
    private void ChessBoard_MouseLeftButtonDown(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(ChessBoard).Properties.IsLeftButtonPressed == false) return;

        if (ViewModel == null) return;
        if (ViewModel.FirstClick)
        {
            _sourcePiece = e.Source as SKBitmapControl;
            if (_sourcePiece?.Name == null) return;
            _sourceCoordinates = GameHelpers.GetCoordinateFromSquare(_sourcePiece.Name);
        }
        else
        {
            Control destinationTile = e.Source switch
            {
                Rectangle tile => tile,
                SKBitmapControl tile => tile,
                _ => throw new ArgumentOutOfRangeException()
            };
            var piece = ViewModel.Game.m_board.GetPiece(_sourceCoordinates);
            if (piece == null) return;
            try
            {
                if (destinationTile.Name == null) return;
                ViewModel.Game.MakeMove(piece, destinationTile.Name);
                var destinationBitmap = ChessBoard.Children.OfType<SKBitmapControl>()
                    .FirstOrDefault(x => x.Name == destinationTile.Name);
                if (destinationBitmap != null) destinationBitmap.Bitmap = _sourcePiece?.Bitmap;

                if (_sourcePiece != null) _sourcePiece.Bitmap = null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        ViewModel.FirstClick = !ViewModel.FirstClick;
    }
}