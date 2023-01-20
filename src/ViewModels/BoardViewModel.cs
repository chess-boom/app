using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

public class BoardViewModel : BaseViewModel
{
    public Game Game { get; }
    public bool FirstClick = true;

    public BoardViewModel(IScreen hostScreen) : base(hostScreen)
    {
        Game = new Game();
    }
}
