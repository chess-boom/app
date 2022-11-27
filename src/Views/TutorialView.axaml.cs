using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class TutorialView : ReactiveUserControl<TutorialViewModel>
{
    public TutorialView()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
    }
}
