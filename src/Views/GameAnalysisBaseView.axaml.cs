using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class GameAnalysisBaseView : ReactiveUserControl<GameAnalysisBaseViewModel>
{
    public GameAnalysisBaseView()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
    }
}