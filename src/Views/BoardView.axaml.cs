using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using Avalonia.Svg.Skia;
using ReactiveUI;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia;
using ChessBoom.Models.Game;
using System;
using Avalonia.Media.Imaging;

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
        placeAllPieces();
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

    private void createChessPiece(char pieceName, int row, int column)
    {
        string source = "";
        switch (pieceName) {
            case 'r': source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/b_.png";
                break;
            case 'n': source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/n_.png";
                break;
            case 'b': source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/b_.png";
                break;
            case 'q': source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/q_.png";
                break;
            case 'k': source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/k_.png";
                break;
            case 'p': source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/p_.png";
                break;
            case 'R':
                source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/R.png";
                break;
            case 'N':
                source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/N.png";
                break;
            case 'B':
                source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/B.png";
                break;
            case 'Q':
                source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/Q.png";
                break;
            case 'K':
                source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/K.png";
                break;
            case 'P':
                source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/P.png";
                break;
            default: source = "C:/Users/agros/Documents/Concordia/Capstone/App/src/Assets/Pieces/B.png";
                break;
        }
        var chessPiece = new Image
        {
            Source = new Bitmap(source)
        };
        chessPiece.Width = 40;
        chessPiece.Height = 40;
        Grid.SetRow(chessPiece, row);
        Grid.SetColumn(chessPiece, column);
        chessBoard.Children.Add(chessPiece);
    }

    private void placeAllPieces()
    {
        string blackPieces = "rnbqkbnrpppppppp";
        int x = 0;
        string whitePieces = "PPPPPPPPRNBQKBNR";
        for(int i=0; i < 2; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                createChessPiece(blackPieces[x], i, j);
                x++;
            }
        }
        x = 0;
        for (int i = 6; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                createChessPiece(whitePieces[x], i, j);
                x++;
            }
        }
    }
}