using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.Models.Game;
using ChessBoom.ViewModels;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
    }

    public void OnClickGameAnalysis(object? sender, RoutedEventArgs e)
    {
        System.Console.WriteLine("Game Analysis clicked");
        var game = new Game();
        System.Console.WriteLine("Game Analysis done");
    }

    public void OnClickVariantGameAnalysis(object? sender, RoutedEventArgs e)
    {
        System.Console.WriteLine("Variant Game Analysis clicked");
        System.Console.WriteLine("variant Game Analysis done");
    }

    public void OnClickTutorial(object? sender, RoutedEventArgs e)
    {
        System.Console.WriteLine("Tutorial clicked");
        System.Console.WriteLine("Tutorial done");
    }
}