using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ChessBoom.Models.Analysis;
using ReactiveUI;
using System.Text.Json;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class GameAnalysisViewModel : BaseViewModel
{
    private ObservableCollection<Player> _playerCollection = new();

    public ObservableCollection<Player> PlayerCollection
    {
        get => _playerCollection;
        set => this.RaiseAndSetIfChanged(ref _playerCollection, value);
    }

    public GameAnalysisViewModel(IScreen hostScreen) : base(hostScreen)
    {
        PlayerCollection = GetDummyPlayerData("Resources/GameAnalysis_Standard.json");
    }

    protected static ObservableCollection<Player> GetDummyPlayerData(string path)
    {
        var jsonString = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ObservableCollection<Player>>(jsonString)!;
    }
}