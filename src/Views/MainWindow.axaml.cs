using System.Diagnostics.CodeAnalysis;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using ReactiveUI;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
    }
}
