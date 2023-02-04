using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Skia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    private SKBitmapControl? _sourcePieceBitmapControl;
    private Rectangle? _sourceTile;
    private IBrush? _sourceColor;
    private List<string> _legalMoves = new();

    private (int, int) _arrowFrom;
    private (int, int) _arrowTo;
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
    /// Load BoardView into the ViewModel
    /// </summary>
    public BoardView()
    {
        this.WhenActivated(_ =>
        {
            DrawChessBoard();
            DrawGridLabels();
            DrawPieces();
            Debug();
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

            _arrowFrom = (position.Item1, GameHelpers.k_boardHeight - position.Item2 - 1);
            textBlock1.Text = $"Square from: {_arrowFrom}";
        };

        ChessBoard.PointerMoved += (sender, e) =>
        {
            if (!e.GetCurrentPoint(ChessBoard).Properties.IsRightButtonPressed) return;

            var position = e.GetCurrentPoint(ChessBoard).Position;

            var row = (int)(position.X / Tile.Width);
            var col = (int)(position.Y / Tile.Height);

            // only if cursor is on ChessBoard
            if (row < 0 || row >= GameHelpers.k_boardWidth || col < 0 || col >= GameHelpers.k_boardHeight) return;

            _arrowTo = (row, col);
            textBlock2.Text = $"Square to: {_arrowTo}";
        };

        ChessBoard.PointerReleased += (sender, e) =>
        {
            // if (!e.GetCurrentPoint(ChessBoard).Properties.IsRightButtonPressed) return;

            if (_arrowMap.ContainsKey(_arrowFrom))
            {
                _arrowMap.Remove(_arrowFrom);
            }
            else
            {
                _arrowMap.Add(_arrowFrom, _arrowTo);
                var arrowPath = CreateArrowPath(_arrowFrom, _arrowTo);
                var path = new Path
                    { Data = arrowPath, Stretch = Stretch.Fill, Fill = Brushes.LimeGreen, Opacity = 0.8 };
                Arrows.Children.Add(path);
            }

            Console.WriteLine($"ArrowMap: {string.Join(", ", _arrowMap.Select(x => $"{x.Key} -> {x.Value}"))}");
        };
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