using System;
using System.Collections.Generic;
using ChessBoom.Models.Game;
using ChessBoom.Models.Game.Pieces;
using ReactiveUI;

namespace ChessBoom.ViewModels;

public class BoardViewModel : BaseViewModel
{
    public Game game;
    public List<Piece> pieces { get; }
    public string c => "p";

    public BoardViewModel(IScreen hostScreen) : base(hostScreen)
    {
        game = new Game();
        pieces = game.m_board.m_pieces;
    }

    public string GetString(Piece p)
    {
        return p.ToString() ?? throw new InvalidOperationException();
    }

    public List<Piece> GetPieces()
    {
        return pieces;
    }

}
