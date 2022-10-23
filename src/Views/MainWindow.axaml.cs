using Avalonia.Controls;
using Avalonia.Interactivity;
using ChessBoom.GameBoard;
using System;

namespace ChessBoom.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnClickGameAnalysis(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("Game Analysis clicked");
            Game game = new Game();
            System.Console.WriteLine("Game Analysis done");
        }

        public void OnClickVariantGameAnalysis()
        {
            
        }

        public void OnClickTutorial()
        {
            
        }
    }
}