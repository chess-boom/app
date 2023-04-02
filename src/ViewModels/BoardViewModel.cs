using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Game;
using ChessBoom.Models.Game.Pieces;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class BoardViewModel : BaseViewModel
{
    public GameHandler GameHandler { get; set; }
    public string Title { get; set; }

    private ObservableCollection<Piece> _capturedPieces = new();
    
    public ObservableCollection<Piece> CapturedPieces
    {
        get => _capturedPieces;
        set => this.RaiseAndSetIfChanged(ref _capturedPieces, value);
    }

    public BoardViewModel(IScreen hostScreen, Variant variant = Variant.Standard) : base(hostScreen)
    {
        Title = variant == Variant.Standard ? "Board" : $"Board: {variant}";
        GameHandler = new GameHandler(variant);
    }
}
