using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class GameAnalysisView : ReactiveUserControl<GameAnalysisViewModel>
{
    private DataGrid _playerData;
    
    public GameAnalysisView()
    {
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);
        
        _playerData = this.FindControl<DataGrid>("PlayerData");
    }
    
    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not GameAnalysisViewModel viewModel) return;
        
        _playerData.DataContext = viewModel;
        _playerData.Items = viewModel.PlayerDataList;
    }
}