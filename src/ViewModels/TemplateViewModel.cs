using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Profile;
using ReactiveUI;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using ChessBoom.Models.Game;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class TemplateViewModel : BaseViewModel
{
    public Profile Profile { get; set; }
    public TemplateViewModel(IScreen hostScreen) : base(hostScreen)
    {
        string username = "MatteoGisondi";
        Profile = new Profile(username);
        ParseGames();
        
    }

    private void ParseGames()
    {
        DirectoryInfo cboom = new DirectoryInfo("../CBoom");
        //sorting files by creation time to get most recent games first
        FileInfo[] files = cboom.GetFiles("*.pgn").OrderByDescending(p => p.CreationTime).ToArray();

        foreach (FileInfo filePath in files)
        {
            String file = filePath.FullName;
            Dictionary<string, string> game = GameHandler.ReadPGN(file);
            Profile.addGame(game);
        }

        Profile.calculateStats();
        //Profile.displayProfileStats();
    }
}