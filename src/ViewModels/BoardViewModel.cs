using System;
using System.Collections.Generic;
using ChessBoom.Models.Game;
using ChessBoom.Models.Game.Pieces;
using ReactiveUI;

namespace ChessBoom.ViewModels;

public class BoardViewModel : BaseViewModel
{
    private Game game;

    public BoardViewModel(IScreen hostScreen) : base(hostScreen)
    {
        game = new Game();
    }
}
