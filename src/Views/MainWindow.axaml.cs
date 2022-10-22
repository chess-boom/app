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
            System.Console.WriteLine("hello");
            Game game = new Game();
            System.Console.WriteLine("game created");
        }

        public void OnClickVariantGameAnalysis()
        {
            
        }

        public void OnClickTutorial()
        {
            
        }
    }
}