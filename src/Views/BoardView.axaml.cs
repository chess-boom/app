using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using Avalonia.Svg.Skia;
using ReactiveUI;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia;

namespace ChessBoom.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    //private Grid chessBoard;
    public BoardView()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
        chessBoard = this.Find<Grid>("chessBoard");
        DrawChessBoard();
    }

    private void DrawChessBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var rect = new Rectangle();
                    rect.Width = 50;
                    rect.Height = 50;
                    rect.StrokeThickness = 0;
                    if ((i + j) % 2 == 0)
                    {
                        rect.Fill = new SolidColorBrush(Color.FromRgb(243,219,180));
                    }
                    else
                    {
                        rect.Fill = new SolidColorBrush(Color.FromRgb(179,140,99));
                    }
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    chessBoard.Children.Add(rect);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                chessBoard.RowDefinitions.Add(new RowDefinition());
                chessBoard.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(49, GridUnitType.Pixel) });
            }
        }
}