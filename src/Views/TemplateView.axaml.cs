using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class TemplateView : ReactiveUserControl<TemplateViewModel>
{
    public TemplateView()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
    }
}
