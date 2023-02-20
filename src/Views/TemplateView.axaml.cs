using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using ChessBoom.Models.Game;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class TemplateView : ReactiveUserControl<TemplateViewModel>
{
    private List<Dictionary<string, string>> games = new List<Dictionary<string, string>>();

    public int totalGames = 0;

    public TemplateView()
    {
        this.WhenActivated(_ =>
        {
            ParseGames();
        });
        AvaloniaXamlLoader.Load(this);
        ToolTip.SetIsOpen(this, true);
    }

    private void ParseGames()
    {
        if (ViewModel is null) return;
        DirectoryInfo cboom = new DirectoryInfo("../CBoom");
        //sorting files by creation time to get most recent games first
        FileInfo[] files = cboom.GetFiles("*.pgn").OrderByDescending(p => p.CreationTime).ToArray();

        foreach (FileInfo filePath in files)
        {
            String file = filePath.FullName;
            Dictionary<string, string> game = GameHandler.ReadPGN(file);
            ViewModel.Profile.addGame(game);
        }

        ViewModel.Profile.calculateStats();
        ViewModel.Profile.displayProfileStats();
    }
}