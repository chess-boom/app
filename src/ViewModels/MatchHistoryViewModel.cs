using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ChessBoom.Models.Analysis;
using ReactiveUI;
using System.Text.Json;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class MatchHistoryViewModel : BaseViewModel
{
    private ObservableCollection<MatchData> _playerCollection = new();

    public ObservableCollection<MatchData> PlayerCollection
    {
        get => _playerCollection;
        set => this.RaiseAndSetIfChanged(ref _playerCollection, value);
    }

    public MatchHistoryViewModel(IScreen hostScreen) : base(hostScreen)
    {
        PlayerCollection =
            GetDummyPlayerData(Path.Combine(System.AppContext.BaseDirectory, "Resources/GameAnalysis_Standard.json"));
    }

    protected static ObservableCollection<MatchData> GetDummyPlayerData(string path)
    {
        var jsonString = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ObservableCollection<MatchData>>(jsonString)!;
    }
}