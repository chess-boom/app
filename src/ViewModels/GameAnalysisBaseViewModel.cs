using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reactive;
using Avalonia.Controls;
using ChessBoom.Models.Game;
using ReactiveUI;
using static System.Net.Mime.MediaTypeNames;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class GameAnalysisBaseViewModel : BoardViewModel
{
    public string Player1 { get; set; }
    public string Player2 { get; set; }
    public List<string> Moves { get; set; }
    public List<string> Pieces { get; set; }
    public List<string> LostPieces { get; set; }
    public List<string> CapturedPieces { get; set; }
    public ReactiveCommand<Unit, Unit> OpenExplorerCommand { get; }
    public string FileData { get; set; }

    public GameAnalysisBaseViewModel(IScreen hostScreen, Variant variant = Variant.Standard) : base(hostScreen)
    {
        Title = variant == Variant.Standard ? "Board" : $"Board: {variant}";
        GameHandler = new GameHandler(variant);
        //FileData = "No File Selected";
        OpenExplorerCommand = handleFileExplorer();
    }

    public static ReactiveCommand<Unit, Unit> handleFileExplorer()
    {
        return ReactiveCommand.CreateFromTask(async () =>
        {
            var parentWindow = new Window();
            var dialog = new OpenFileDialog();
            dialog.Title = "Select a file";
            dialog.Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
            dialog.Filters.Add(new FileDialogFilter() { Name = "PGN Files", Extensions = { "pgn" } });
            var result = await dialog.ShowAsync(parentWindow);
            if (result != null)
            {
                var selectedFilePath = dialog.InitialFileName;
                //FileData = selectedFilePath;
                var file = result.FirstOrDefault();
                if (file != null)
                {
                    // Read the file data
                    //FileData = selectedFilePath;//await System.IO.File.ReadAllTextAsync(selectedFilePath);
                }
            }
            else
            {
                // User clicked "Cancel" or closed the dialog
                //FileData = "Canceled";
            }
        });
    }
}