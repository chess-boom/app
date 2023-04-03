using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
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
public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    private bool _boardLocked;

    private SKBitmapControl? _sourcePieceBitmapControl;
    private Rectangle? _sourceTile;
    private IBrush? _sourceColor;
    private List<string> _legalMoves = new();

    private const int PromotionPiecesOffset = 2;

    /// <summary>
    /// Defines attributes related to rendered Tiles
    /// </summary>
    private abstract class Tile
    {
        internal const int k_width = 50;
        internal const int k_height = 50;

        internal static readonly Color k_white = Color.FromRgb(243, 219, 180);
        internal static readonly Color k_black = Color.FromRgb(179, 140, 99);
        internal static readonly Color k_highlight = Color.FromRgb(102, 178, 255);
    }

    /// <summary>
    /// Defines attributes related to rendered Dots
    /// </summary>
    private abstract class Dot
    {
        internal static int Width => Tile.k_width / 5;
        internal static int Height => Tile.k_height / 5;

        internal static readonly Color k_green = Color.FromRgb(0, 153, 0);
        internal static readonly Color k_red = Color.FromRgb(153, 0, 0);
    }

    /// <summary>
    /// Defines attributes related to rendered Pieces
    /// </summary>
    internal static class Piece
    {
        internal static readonly string k_white = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets/Pieces/{0}.svg");
        internal static readonly string k_black = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets/Pieces/{0}_.svg");
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
            ViewModel.WhenAnyValue(x => x.GameHandler)
            .Subscribe(_ => DrawPieces());
        });
        AvaloniaXamlLoader.Load(this);

        ChessBoard = this.Find<Grid>("ChessBoard");
        NumberGridLabels = this.Find<Grid>("NumberGridLabels");
        LetterGridLabels = this.Find<Grid>("LetterGridLabels");
        PromotionPieces = this.Find<Grid>("PromotionPieces");
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
                    Width = Tile.k_width,
                    Height = Tile.k_height,
                    Name = square,
                    StrokeThickness = 0,
                    Fill = (row + col) % 2 == 0 ? new SolidColorBrush(Tile.k_white) : new SolidColorBrush(Tile.k_black)
                };

                Grid.SetRow(tile, row);
                Grid.SetColumn(tile, col);
                ChessBoard.Children.Add(tile);

                var bitmap = new SKBitmapControl
                {
                    Width = Tile.k_width,
                    Height = Tile.k_height,
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
        for (var i = 0; i < GameHelpers.k_boardHeight; i++)
        {
            NumberGridLabels.RowDefinitions.Add(new RowDefinition { Height = new GridLength(Tile.k_height) });
            var number = new TextBlock
            {
                Text = (GameHelpers.k_boardWidth - i).ToString(),
                FontWeight = FontWeight.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var container = new ContentControl
            {
                VerticalAlignment = VerticalAlignment.Center,
                Width = Tile.k_width,
                Content = number
            };
            NumberGridLabels.Children.Add(container);
            Grid.SetRow(container, i);
        }

        for (var i = 0; i < GameHelpers.k_boardWidth; i++)
        {
            LetterGridLabels.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Tile.k_width) });
            var letter = new TextBlock
            {
                Text = ((char)('A' + i)).ToString(),
                FontWeight = FontWeight.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var container = new ContentControl
            {
                VerticalAlignment = VerticalAlignment.Center,
                Height = Tile.k_height,
                Content = letter
            };
            LetterGridLabels.Children.Add(container);
            Grid.SetColumn(container, i);
        }
    }

    /// <summary>
    /// Generate a Bitmap from a Piece's SVG
    /// </summary>
    /// <param name="piecePath"></param>
    /// <param name="width">The width of the Bitmap</param>
    /// <param name="height">The height of the Bitmap</param>
    /// <returns>Bitmap of the requested piece</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static SKBitmap GeneratePieceBitmap(string piecePath, int width = Tile.k_width, int height = Tile.k_height)
    {
        using var svg = new SKSvg();
        var bitmap = new SKBitmap(width, height);
        svg.Load(piecePath);
        if (svg.Picture is null) throw new InvalidOperationException("svg.Picture is null");
        var canvas = new SKCanvas(bitmap);
        var matrix = SKMatrix.CreateScale(
            width / svg.Picture.CullRect.Width,
            height / svg.Picture.CullRect.Height
        );
        canvas.DrawPicture(svg.Picture, ref matrix);
        return bitmap;
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
                    _ => throw new InvalidOperationException("Invalid Player")
                };

                bitmap.Bitmap = GeneratePieceBitmap(piecePath);
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
        if (_boardLocked) return; // prevent user from interacting with the board while selecting a promotion piece

        if (ViewModel is null) return;
        if (!e.GetCurrentPoint(ChessBoard).Properties.IsLeftButtonPressed) return;

        if (_sourcePieceBitmapControl is null)
        {
            _sourcePieceBitmapControl = e.Source as SKBitmapControl;
            _sourceTile = ChessBoard.Children.OfType<Rectangle>()
                .FirstOrDefault(x => x.Name == _sourcePieceBitmapControl?.Name);

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
                // The player may click on a drawn Ellipse object
                Ellipse tile => tile,
                _ => throw new InvalidOperationException("Invalid Tile")
            };

            if (_sourceTile is null) return;

            // un-highlight the source tile
            _sourceTile.Fill = _sourceColor;
            RemoveLegalMoves();

            // if the destination tile is the source tile, do nothing
            if (destinationTile.Name == _sourceTile.Name) return;

            if (_sourceTile?.Name is null || destinationTile.Name is null) return;

            // if the destination tile is not a legal move, do nothing
            if (!_legalMoves.Contains(destinationTile.Name)) return;

            try
            {
                ViewModel.GameHandler.MakeMove(_sourceTile.Name, destinationTile.Name, RequestPromotionPiece);
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
    /// Draw the possible pieces to promote to on the PromotionPieces Grid Control
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void DrawPromotionPieces()
    {
        for (var i = 0; i < PromotionPiecesOffset; i++)
        {
            PromotionPieces.RowDefinitions.Add(new RowDefinition { Height = new GridLength(Tile.k_height) });
        }

        for (var row = 0; row < GameHelpers.k_promotionPieces.Count; row++)
        {
            PromotionPieces.RowDefinitions.Add(new RowDefinition { Height = new GridLength(Tile.k_height) });

            var promotionPiece = new SKBitmapControl
            {
                Name = GameHelpers.k_promotionPieces[row].ToString(),
                Width = Tile.k_width,
                Height = Tile.k_height,
                ZIndex = 1
            };
            var piecePath = ViewModel?.GameHandler.GetPlayerToPlay() switch
            {
                Player.White => string.Format(Piece.k_white, GameHelpers.k_promotionPieces[row]),
                Player.Black => string.Format(Piece.k_black, GameHelpers.k_promotionPieces[row].ToString().ToLower()),
                _ => throw new InvalidOperationException("Invalid Player")
            };
            promotionPiece.Bitmap = GeneratePieceBitmap(piecePath);
            PromotionPieces.Children.Add(promotionPiece);
            Grid.SetRow(promotionPiece, row + PromotionPiecesOffset);
        }
    }

    /// <summary>
    /// Clear the PromotionPieces Grid Control from the screen
    /// </summary>
    private void ClearPromotionPieces()
    {
        PromotionPieces.Children.Clear();
        PromotionPieces.RowDefinitions.Clear();
    }

    /// <summary>
    /// Display a Grid to the User to select a piece to promote to on reaching the last rank
    /// </summary>
    /// <returns>A Task for the User to select a piece to promote to</returns>
    private async Task<char> RequestPromotionPiece()
    {
        _boardLocked = true; // disable the Board

        DrawPromotionPieces();

        var tcs = new TaskCompletionSource<char>();

        void PointerPressedHandler(object? _, PointerPressedEventArgs e)
        {
            var promotionPieceBitmapControl = e.Source as SKBitmapControl;
            var promotionPiece = promotionPieceBitmapControl?.Name?[0];
            if (!promotionPiece.HasValue) return;
            tcs.SetResult(promotionPiece.Value);
            ClearPromotionPieces();
            DrawPieces();
            PromotionPieces.PointerPressed -= PointerPressedHandler;
            _boardLocked = false; // re-enable the Board after the User has selected a piece
        }

        PromotionPieces.PointerPressed += PointerPressedHandler;

        return await tcs.Task;
    }


    /// <summary>
    /// Display the available moves for the selected piece
    /// </summary>
    private void DisplayLegalMoves(List<string> availableMoves)
    {
        foreach (var move in availableMoves)
        {
            // check if the move is a castling move
            if (move is "O-O" or "O-O-O")
            {
                DisplayCastling(move);
                continue;
            }

            Ellipse dot;

            var attackedPiece = ViewModel?.GameHandler.GetPiece(GameHelpers.GetCoordinateFromSquare(move));

            if (attackedPiece is not null)
            {
                dot = new Ellipse
                {
                    Width = Tile.k_width,
                    Height = Tile.k_height,
                    ZIndex = 2,
                    Name = move,
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
                    Name = move
                };
            }

            (int col, int row) coordinates = GameHelpers.GetCoordinateFromSquare(move);
            Grid.SetRow(dot, GameHelpers.k_boardWidth - 1 - coordinates.row);
            Grid.SetColumn(dot, coordinates.col);
            ChessBoard.Children.Add(dot);
        }
    }

    private void DisplayCastling(string move)
    {
        var castle = new Ellipse
        {
            Width = Dot.Width,
            Height = Dot.Height,
            Fill = new SolidColorBrush(Dot.k_green),
            ZIndex = 2,
            Name = move
        };
        (int col, int row) coordinates;
        if (ViewModel?.GameHandler.GetPlayerToPlay() == Player.Black)
        {
            coordinates = move switch
            {
                "O-O" => (6, 7),
                "O-O-O" => (2, 7),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            coordinates = move switch
            {
                "O-O" => (6, 0),
                "O-O-O" => (2, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        Grid.SetRow(castle, GameHelpers.k_boardWidth - 1 - coordinates.row);
        Grid.SetColumn(castle, coordinates.col);
        ChessBoard.Children.Add(castle);
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