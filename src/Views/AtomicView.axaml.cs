using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Skia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ChessBoom.Models.Game;
using ChessBoom.ViewModels;
using ReactiveUI;
using SkiaSharp;
using Svg.Skia;

// ReSharper disable SuggestBaseTypeForParameter

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class AtomicView : ReactiveUserControl<AtomicViewModel>
{
    private SKBitmapControl? _sourcePieceBitmapControl;
    private Rectangle? _sourceTile;
    private IBrush? _sourceColor;
    private List<string> _legalMoves = new();

    /// <summary>
    /// Defines attributes related to rendered Tiles
    /// </summary>
    private abstract class Tile
    {
        internal static int Width => 50;
        internal static int Height => 50;

        internal static readonly Color k_white = Color.FromRgb(243, 219, 180);
        internal static readonly Color k_black = Color.FromRgb(179, 140, 99);
        internal static readonly Color k_highlight = Color.FromRgb(102, 178, 255);
    }

    /// <summary>
    /// Defines attributes related to rendered Dots
    /// </summary>
    private abstract class Dot
    {
        internal static int Width => Tile.Width / 5;
        internal static int Height => Tile.Height / 5;

        internal static readonly Color k_green = Color.FromRgb(0, 153, 0);
        internal static readonly Color k_red = Color.FromRgb(153, 0, 0);
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
    /// Load AtomicView into the ViewModel
    /// </summary>
    public AtomicView()
    {
        this.WhenActivated(_ =>
        {
            DrawChessBoard();
            DrawGridLabels();
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
        for (var row = 0; row < GameHelpers.k_boardHeight; row++)
        {
            ChessBoard.RowDefinitions.Add(new RowDefinition());
            ChessBoard.ColumnDefinitions.Add(new ColumnDefinition());

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
    }

    /// <summary>
    /// Add letters and numbers to the ChessBoard
    /// </summary>
    private void DrawGridLabels()
    {
        ChessBoard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(Tile.Height) });
        ChessBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Tile.Width) });
        for (var i = 0; i < GameHelpers.k_boardWidth; i++)
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

        for (var i = 0; i < GameHelpers.k_boardHeight; i++)
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
        if (ViewModel is null) return;

        foreach (var bitmap in ChessBoard.Children.OfType<SKBitmapControl>())
        {
            if (bitmap.Name is null) return;
            var piece = ViewModel.GameHandler.GetPiece(bitmap.Name);
            if (piece is not null)
            {
                var piecePath = piece.GetPlayer() switch
                {
                    Player.White => string.Format(Piece.k_white, piece),
                    Player.Black => string.Format(Piece.k_black, piece),
                    _ => throw new ArgumentOutOfRangeException()
                };

                using var svg = new SKSvg();
                bitmap.Bitmap = new SKBitmap(Tile.Width, Tile.Height);
                svg.Load(piecePath);
                if (svg.Picture is null) throw new ArgumentNullException();
                var canvas = new SKCanvas(bitmap.Bitmap);
                var matrix = SKMatrix.CreateScale(
                    Tile.Width / svg.Picture.CullRect.Width,
                    Tile.Height / svg.Picture.CullRect.Height
                );
                canvas.DrawPicture(svg.Picture, ref matrix);
            }
            else
            {
                bitmap.Bitmap = null;
            }

            bitmap.InvalidateVisual();
        }
    }

    /// <summary>
    /// Handle LeftButton events for the ChessBoard Grid Control
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    // ReSharper disable once UnusedParameter.Local
    private void ChessBoard_MouseLeftButtonDown(object? sender, PointerPressedEventArgs e)
    {
        if (ViewModel is null) return;
        if (!e.GetCurrentPoint(ChessBoard).Properties.IsLeftButtonPressed) return;

        if (_sourcePieceBitmapControl is null)
        {
            _sourcePieceBitmapControl = e.Source as SKBitmapControl;
            _sourceTile = ChessBoard.Children.OfType<Rectangle>().FirstOrDefault(x => x.Name == _sourcePieceBitmapControl?.Name);

            _sourceColor = _sourceTile?.Fill;
            if (_sourceTile is not null) _sourceTile.Fill = new SolidColorBrush(Tile.k_highlight);

            if (_sourcePieceBitmapControl?.Name is null) return;
            var sourcePiece = ViewModel.GameHandler.GetPiece(_sourcePieceBitmapControl.Name);
            if (sourcePiece?.GetPlayer() != ViewModel?.GameHandler.GetPlayerToPlay()) return;
            if (sourcePiece is not null) _legalMoves = sourcePiece.GetLegalMoves();
            DisplayLegalMoves(_legalMoves);
        }
        else
        {
            _sourcePieceBitmapControl = null;

            Control destinationTile = e.Source switch
            {
                Rectangle tile => tile,
                SKBitmapControl tile => tile,
                Ellipse tile => tile,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (_sourceTile is null) return;

            // un-highlight the source tile
            _sourceTile.Fill = _sourceColor;
            RemoveLegalMoves();

            // if the destination tile is the source tile, do nothing
            if (destinationTile?.Name == _sourceTile.Name) return;

            if (_sourceTile?.Name is null || destinationTile?.Name is null) return;

            // if the destination tile is not a legal move, do nothing
            if (!_legalMoves.Contains(destinationTile.Name)) return;

            try
            {
                ViewModel.GameHandler.MakeMove(_sourceTile.Name, destinationTile.Name);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                DrawPieces();
            }
        }
    }

    /// <summary>
    /// Display the available moves for the selected piece
    /// </summary>
    private void DisplayLegalMoves(List<string> availableMoves)
    {
        foreach (var square in availableMoves)
        {
            Ellipse dot;

            var attackedPiece = ViewModel?.GameHandler.GetPiece(GameHelpers.GetCoordinateFromSquare(square));

            if (attackedPiece is not null)
            {
                dot = new Ellipse
                {
                    Width = Tile.Width,
                    Height = Tile.Height,
                    ZIndex = 2,
                    Name = square,
                    Stroke = new SolidColorBrush(Dot.k_red),
                    StrokeThickness = 3
                };
            }
            else
            {
                dot = new Ellipse
                {
                    Width = Dot.Width,
                    Height = Dot.Height,
                    Fill = new SolidColorBrush(Dot.k_green),
                    ZIndex = 2,
                    Name = square
                };
            }

            (int col, int row) coordinates = GameHelpers.GetCoordinateFromSquare(square);
            Grid.SetRow(dot, GameHelpers.k_boardWidth - 1 - coordinates.row);
            Grid.SetColumn(dot, coordinates.col);
            ChessBoard.Children.Add(dot);
        }
    }

    /// <summary>
    /// Remove drawn dots from the ChessBoard
    /// </summary>
    private void RemoveLegalMoves()
    {
        var dots = ChessBoard.Children.OfType<Ellipse>().ToList();
        foreach (var dot in dots)
        {
            ChessBoard.Children.Remove(dot);
        }
    }
}