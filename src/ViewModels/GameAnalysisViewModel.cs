using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using ChessBoom.Models.Analysis;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class GameAnalysisViewModel : BoardViewModel
{
    private ObservableCollection<MoveEvaluation> _bestMovesCollection;

    public ObservableCollection<MoveEvaluation> BestMovesCollection
    {
        get => _bestMovesCollection;
        set => this.RaiseAndSetIfChanged(ref _bestMovesCollection, value);
    }

    private ObservableCollection<Evaluation> _evaluationCollection;

    public ObservableCollection<Evaluation> EvaluationCollection
    {
        get => _evaluationCollection;
        set => this.RaiseAndSetIfChanged(ref _evaluationCollection, value);
    }

    private SimpleReport? _analysisReport;

    public SimpleReport? AnalysisReport
    {
        get => _analysisReport;
        set => this.RaiseAndSetIfChanged(ref _analysisReport, value);
    }

    private Stockfish _engine;

    private Evaluation? _currentEvaluation;

    private Evaluation? _previousEvaluation;

    public ReactiveCommand<Unit, Unit> OpenExplorerCommand { get; }
    public string? FileData { get; set; }

    private Variant _variant;

    public GameAnalysisViewModel(IScreen hostScreen, Variant variant = Variant.Standard) : base(hostScreen)
    {
        Title = "";

        _variant = variant;

        _bestMovesCollection = new ObservableCollection<MoveEvaluation>();

        _engine = new Stockfish
        {
            Variant = variant,
            FenPosition = GameHandler.GetCurrentFENPosition()
        };

        _evaluationCollection = new ObservableCollection<Evaluation>();

        GameHandler = new GameHandler(variant);

        GameHandler.MovePlayed += UpdateEngine;
        GameHandler.MovePlayed += UpdateGameData;
        GameHandler.MovePlayed += UpdateAnalysisData;

        OpenExplorerCommand = HandleFileExplorer();
    }

    private void UpdateEngine(string startingSquare, string destinationSquare)
    {
        _engine.FenPosition = GameHandler.GetCurrentFENPosition();
        if (_currentEvaluation is not null)
            _previousEvaluation = _currentEvaluation;
        _currentEvaluation = _engine.GetStaticEvaluation() ?? throw new InvalidOperationException();
        _evaluationCollection.Add(_currentEvaluation);
    }

    private void UpdateGameData(string _, string destinationsquare)
    {
        BestMovesCollection = new ObservableCollection<MoveEvaluation>(_engine.GetNBestMoves(10));
    }

    private void UpdateAnalysisData(string startingsquare, string destinationsquare)
    {
        AnalysisReport = SimpleReport.GetSimpleReport(_currentEvaluation,
            _previousEvaluation,
            GameHandler.GetPlayerToPlay());
    }

    private ReactiveCommand<Unit, Unit> HandleFileExplorer()
    {
        return ReactiveCommand.CreateFromTask(async () =>
        {
            var parentWindow = new Window();
            var dialog = new OpenFileDialog
            {
                Title = "Select a file",
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads"
            };
            dialog.Filters?.Add(new FileDialogFilter { Name = "PGN Files", Extensions = { "pgn" } });
            var result = await dialog.ShowAsync(parentWindow);
            if (result != null)
            {
                var selectedFilePath = System.IO.Path.GetDirectoryName(dialog.InitialFileName) + "\\"+ dialog.InitialFileName;
                HandleGameFileLoading(selectedFilePath);
                //var file = result.FirstOrDefault();
            }
        });
    }

    private void HandleGameFileLoading(string filePath)
    {
        if(filePath != null)
        {
            GameHandler = new GameHandler(_variant);
            GameHandler.LoadGame(filePath);
            var engine = new Stockfish
                    {
                        Variant = _variant,
                        FenPosition = GameHandler.GetCurrentFENPosition()
                    };
            engine.GetStaticEvaluation();
        }
        else
        {
            throw new FileNotFoundException("Error! PGN file not found!");
        }
    }
}