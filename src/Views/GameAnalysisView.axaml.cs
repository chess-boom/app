using System;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Skia;
using ChessBoom.Models.Game;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class GameAnalysisView : ReactiveUserControl<GameAnalysisViewModel>
{
    private abstract class CapturedPieceTile
    {
        internal const int k_width = 10;
        internal const int k_height = 10;
    }

    public GameAnalysisView()
    {
        this.WhenActivated(_ => { InitializeCapturedPieces(); });
        AvaloniaXamlLoader.Load(this);

        CapturedPieces = this.Find<Grid>("CapturedPieces");
        EvaluationDataGrid = this.FindControl<DataGrid>("EvaluationDataGrid");

        if (ViewModel is null) return;
        
        ViewModel.GameHandler.MovePlayed += (_, _) => { DrawCapturedPieces(); };
        ViewModel.EvaluationCollection.CollectionChanged += (_, _) =>
        {
            EvaluationDataGrid.ScrollIntoView(ViewModel.EvaluationCollection.Last(), null);
        };
    }

    private void InitializeCapturedPieces()
    {
        var pieceCount = ViewModel?.GameHandler.GetPieces().Count;
        for (var i = 0; i < GameHelpers.k_boardWidth; i++)
        {
            CapturedPieces.ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (var i = 0; i < GameHelpers.k_boardWidth * GameHelpers.k_boardHeight / pieceCount; i++)
        {
            CapturedPieces.RowDefinitions.Add(new RowDefinition());
        }
    }

    private void DrawCapturedPieces()
    {
        CapturedPieces.Children.Clear();
        var row = 0;
        var column = 0;
        foreach (var piece in ViewModel?.GameHandler.GetCapturedPieces()!)
        {
            SKBitmapControl bitmapControl = new();

            var piecePath = piece.GetPlayer() switch
            {
                Player.White => string.Format(BoardView.Piece.k_white, piece),
                Player.Black => string.Format(BoardView.Piece.k_black, piece),
                _ => throw new InvalidOperationException("Invalid Player")
            };

            bitmapControl.Bitmap =
                BoardView.GeneratePieceBitmap(piecePath, CapturedPieceTile.k_width, CapturedPieceTile.k_height);

            CapturedPieces.Children.Add(bitmapControl);

            Grid.SetColumn(bitmapControl, column++);
            if (column == GameHelpers.k_boardWidth)
            {
                column = 0;
                row++;
            }

            Grid.SetRow(bitmapControl, row);
        }
    }
}