using System;
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

        internal static readonly Color White = Color.FromRgb(243, 219, 180);
        internal static readonly Color Black = Color.FromRgb(179, 140, 99);
    }

    private abstract class Piece
    {
        internal const string White = "Assets/Pieces/{0}.svg";
        internal const string Black = "Assets/Pieces/{0}_.svg";
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
                    Fill = (i + j) % 2 == 0 ? new SolidColorBrush(Tile.White) : new SolidColorBrush(Tile.Black)
                };
                Grid.SetRow(tile, i);
                Grid.SetColumn(tile, j);
                ChessBoard.Children.Add(tile);
            }
        }
    }

    private void DrawPieces()
    {
        // In Progress
        // using var bitmap = new SKBitmap(Tile.Width, Tile.Height);
        // using var canvas = new SKCanvas(bitmap);
        // var piece = new Pawn(ViewModel!.game.m_board, Player.Black, (0, 0));
        // var piecePath = piece.GetPlayer() switch
        // {
        //     Player.White => string.Format(Piece.White, piece),
        //     Player.Black => string.Format(Piece.Black, piece),
        //     _ => throw new ArgumentOutOfRangeException()
        // };
        // var svg = new SKSvg();
        // svg.Load(piecePath);
        // canvas.DrawPicture(svg.Picture);
        
        // Grid.SetRow(canvas, 0);
        // Grid.SetColumn(canvas, 0);
        
        // foreach (var piece in ViewModel.game.m_board.m_pieces)
        // {
        //     var tile = this.Find<Rectangle>(GameHelpers.GetSquareFromCoordinate(piece.GetCoordinates()));
        //     var
        // }
    }
}