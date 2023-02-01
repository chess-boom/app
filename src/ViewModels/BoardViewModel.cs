using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class BoardViewModel : BaseViewModel
{
    public GameHandler GameHandler { get; }

    public BoardViewModel(IScreen hostScreen) : base(hostScreen)
    {
        GameHandler = new GameHandler();
    }
}
