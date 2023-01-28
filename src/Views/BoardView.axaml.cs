using System;
using System.Linq;
using Avalonia;
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
using System.Collections.Generic;

namespace ChessBoom.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    private (int, int) _from;
    private (int, int) _to;
    private Dictionary<(int, int), (int, int)> _arrowMap = new Dictionary<(int, int), (int, int)>();

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
            Debug();
        });
        AvaloniaXamlLoader.Load(this);
        // WARN : VS Code Complains: `The name 'ChessBoard' does not exist in the current context [ChessBoom]csharp(CS0103)`
        // ChessBoard is defined at build time in Avalonia.NameGenerator/Avalonia.NameGenerator.AvaloniaNameSourceGenerator/BoardView.g.cs
        ChessBoard = this.Find<Grid>("ChessBoard");
    }

    // A method named Debug that adds a list box to the view and binds in to mouse events
    private void Debug()
    {
        var textBlock1 = this.Find<TextBlock>("tb1");
        var textBlock2 = this.Find<TextBlock>("tb2");

        Arrows = this.Find<Canvas>("Arrows");

        ChessBoard.PointerPressed += (sender, e) =>
        {
            if (!e.GetCurrentPoint(ChessBoard).Properties.IsRightButtonPressed) return;

            var source = e.Source as Control;
            if (source?.Name == null) return;
            var position = GameHelpers.GetCoordinateFromSquare(source.Name);

            _from = (position.Item1, GameHelpers.k_boardHeight - position.Item2 - 1);
            textBlock1.Text = $"Square from: {_from}";
        };

        ChessBoard.PointerMoved += (sender, e) =>
        {
            if (!e.GetCurrentPoint(ChessBoard).Properties.IsRightButtonPressed) return;

            var position = e.GetCurrentPoint(ChessBoard).Position;

            var row = (int)(position.X / Tile.Width);
            var col = (int)(position.Y / Tile.Height);

            // only if cursor is on ChessBoard
            if (row < 0 || row >= GameHelpers.k_boardWidth || col < 0 || col >= GameHelpers.k_boardHeight) return;

            _to = (row, col);
            textBlock2.Text = $"Square to: {_to}";
        };

        ChessBoard.PointerReleased += (sender, e) =>
        {
            // if (!e.GetCurrentPoint(ChessBoard).Properties.IsRightButtonPressed) return;

            if (_arrowMap.ContainsKey(_from))
            {
                _arrowMap.Remove(_from);
            }
            else
            {
                _arrowMap.Add(_from, _to);
                var arrowPath = CreateArrowPath(_from, _to);
                var path = new Path
                    { Data = arrowPath, Stretch = Stretch.Fill, Fill = Brushes.LimeGreen, Opacity = 0.8 };
                Arrows.Children.Add(path);
            }

            Console.WriteLine($"ArrowMap: {string.Join(", ", _arrowMap.Select(x => $"{x.Key} -> {x.Value}"))}");
        };
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

                var name = new TextBlock
                {
                    Name = square,
                    Text = square,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 10,
                };
                Grid.SetRow(name, row);
                Grid.SetColumn(name, col);
                ChessBoard.Children.Add(name);

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

            bitmap.Tag = piece;

            bitmap.InvalidateVisual();
        }
    }

    private PathGeometry CreateArrowPath((int x, int y) from, (int x, int y) to)
    {
        var tileWidth = Tile.Width;
        var tileHeight = Tile.Height;
        
        var startPoint = new Point(from.x * tileWidth + tileWidth / 2, from.y * tileHeight + tileHeight / 2);
        var endPoint = new Point(to.x * tileWidth + tileWidth / 2, to.y * tileHeight + tileHeight / 2);
        var direction = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
        direction.Normalize();
        var lineEnd = endPoint - direction;
        var angle = Math.Atan2(direction.Y, direction.X);

        var pathGeometry = new PathGeometry();
        var pathFigure = new PathFigure
        {
            StartPoint = startPoint
        };
        pathGeometry.Figures.Add(pathFigure);

        var lineSegment1 = new LineSegment
        {
            Point = new Point(lineEnd.X - 30 * Math.Sin(angle),
                lineEnd.Y + 30 * Math.Cos(angle))
        };
        pathFigure.Segments.Add(lineSegment1);

        var lineSegment2 = new LineSegment
        {
            Point = new Point(lineEnd.X - 15 * Math.Sin(angle - Math.PI / 6),
                lineEnd.Y + 15 * Math.Cos(angle - Math.PI / 6))
        };
        pathFigure.Segments.Add(lineSegment2);

        var lineSegment3 = new LineSegment
        {
            Point = new Point(lineEnd.X - 15 * Math.Sin(angle + Math.PI / 6),
                lineEnd.Y + 15 * Math.Cos(angle + Math.PI / 6))
        };
        pathFigure.Segments.Add(lineSegment3);

        var lineSegment4 = new LineSegment
        {
            Point = lineEnd
        };
        pathFigure.Segments.Add(lineSegment4);

        return pathGeometry;
    } 

    // a method that on right click draws an arrow from the selected square to the clicked square
    private void ChessBoard_OnPointerPressed(object? sender, PointerEventArgs e)
    {
        if (!e.GetCurrentPoint(ChessBoard).Properties.IsRightButtonPressed) return;

        var clickedX = (int)(e.GetPosition(ChessBoard).X / Tile.Width);
        int clickedY = GameHelpers.k_boardHeight - (int)(e.GetPosition(ChessBoard).Y / Tile.Height) - 1;
        var clickedSquare = GameHelpers.GetSquareFromCoordinate((clickedY, clickedX));
        var selectedSquare = GameHelpers.GetSquareFromCoordinate(
            (GameHelpers.k_boardHeight - (int)(e.GetPosition(ChessBoard).Y / Tile.Height) - 1,
                (int)(e.GetPosition(ChessBoard).X / Tile.Width))
        );

        var clickedBitmap = ChessBoard.Children.OfType<SKBitmapControl>().FirstOrDefault(x => x.Name == clickedSquare);
        var selectedBitmap =
            ChessBoard.Children.OfType<SKBitmapControl>().FirstOrDefault(x => x.Name == selectedSquare);

        if (clickedBitmap == null || selectedBitmap == null) throw new ArgumentNullException();

        var canvas = new SKCanvas(clickedBitmap.Bitmap);
        var paint = new SKPaint
        {
            Color = SKColors.Red,
            StrokeWidth = 5,
            IsStroke = true,
            IsAntialias = true
        };
        canvas.DrawLine(
            new SKPoint((float)selectedBitmap.Width / 2, (float)selectedBitmap.Height / 2),
            new SKPoint((float)clickedBitmap.Width / 2, (float)clickedBitmap.Height / 2),
            paint
        );
        clickedBitmap.InvalidateVisual();
    }
}