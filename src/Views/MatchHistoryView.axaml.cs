using System.Diagnostics;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class PlayerAnalysisView : ReactiveUserControl<MatchHistoryViewModel>
{
    public PlayerAnalysisView()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
    }

    private void HandleRowClick(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        var row = e.Row;
        Debug.WriteLine(row);
    }
}