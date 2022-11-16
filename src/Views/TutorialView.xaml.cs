using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.Models.Game;
using ChessBoom.ViewModels;
using ReactiveUI;

namespace ChessBoom;

public class TutorialView : ReactiveUserControl<DashboardViewModel>
{
    public TutorialView()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
    }
}
