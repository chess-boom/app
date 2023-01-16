using Avalonia.Controls.Shapes;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ChessBoom.Models.Game.Pieces;
using ChessBoom.Models.Game;
using ChessBoom.ViewModels;
using ReactiveUI;
using SkiaSharp;
using Svg.Skia;

namespace ChessBoom.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    private abstract class Tile
    {
        internal static int Width => 50;
        internal static int Height => 50;

        internal static readonly Color WHITE = Color.FromRgb(243, 219, 180);
        internal static readonly Color BLACK = Color.FromRgb(179, 140, 99);
    }

    public BoardView()
    {
        ChessBoard = this.Find<Grid>("ChessBoard");
        this.WhenActivated(_ =>
        {
            DrawChessBoard();
            DrawPieces();
        });
        AvaloniaXamlLoader.Load(this);
    }

    private void DrawChessBoard()
    {
        for (var i = 0; i < GameHelpers.k_boardWidth; i++)
        {
            for (var j = 0; j < GameHelpers.k_boardHeight; j++)
            {
                var tile = new Rectangle
                {
                    Width = Tile.Width,
                    Height = Tile.Height,
                    Name = GameHelpers.GetSquareFromCoordinate((i, j)),
                    StrokeThickness = 0,
                    Fill = (i + j) % 2 == 0 ? new SolidColorBrush(Tile.WHITE) : new SolidColorBrush(Tile.BLACK)
                };
                Grid.SetRow(tile, i);
                Grid.SetColumn(tile, j);
                ChessBoard.Children.Add(tile);
            }
        }

        for (var i = 0; i < GameHelpers.k_boardWidth; i++)
        {
            ChessBoard.RowDefinitions.Add(new RowDefinition());
            ChessBoard.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(Tile.Width, GridUnitType.Pixel)
                }
            );
        }
    }

    private void DrawPieces()
    {
        // In Progress
        // using (var bitmap = new SKBitmap(width, height))
        // using (var canvas = new SKCanvas(bitmap))
        // {
        //     var piece = new Pawn(ViewModel.game.m_board, Player.White, (0, 0));
        //     var PiecePath = piece.GetPlayer() == Player.White
        //         ? $"Assets/Pieces/{piece}.svg"
        //         : $"Assets/Pieces/{piece}_.svg";
        //     var svg = new SKSvg();
        //     if (svg.Load(PiecePath) is { })
        //     {
        //         var svgControl = new SvgControl();
        //         svgControl.Svg = svg;
        //     }

        //     Svg.Draw(canvas, svg);
        //     var image = SKImage.FromBitmap(bitmap);
        // }
        // foreach (var piece in ViewModel.game.m_board.m_pieces)
        // {
        //     var tile = this.Find<Rectangle>(GameHelpers.GetSquareFromCoordinate(piece.GetCoordinates()));
        //     var
        // }
    }
}