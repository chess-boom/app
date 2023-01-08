using Avalonia.Markup.Xaml;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
    }
}