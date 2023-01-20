using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

public class BoardViewModel : BaseViewModel
{
    public Game Game { get; }

    public BoardViewModel(IScreen hostScreen) : base(hostScreen)
    {
        Game = new Game();
    }
}
