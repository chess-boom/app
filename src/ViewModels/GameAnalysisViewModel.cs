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

    private SimpleReport? _analysisReport = new();

    public SimpleReport? AnalysisReport
    {
        get => _analysisReport;
        private set => this.RaiseAndSetIfChanged(ref _analysisReport, value);
    }

    private string? _currentOpening = "Opening: ";

    public string? CurrentOpening
    {
        get => _currentOpening;
        set => this.RaiseAndSetIfChanged(ref _currentOpening, value);
    }

    private readonly Stockfish _engine;

    private Evaluation? _currentEvaluation;

    private Evaluation? _previousEvaluation;

    public ReactiveCommand<Unit, Unit> OpenExplorerCommand { get; }
    public string? FileData { get; set; }

    private readonly Variant _variant;

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

        GameHandler.MovePlayed += (_, _) =>
        {
            UpdateEngine();
            UpdateGameData();
            UpdateAnalysisData();
            UpdateCurrentOpening();
        };

        OpenExplorerCommand = HandleFileExplorer();
    }

    private void UpdateEngine()
    {
        _engine.FenPosition = GameHandler.GetCurrentFENPosition();
        if (_currentEvaluation is not null)
            _previousEvaluation = _currentEvaluation;
        _currentEvaluation = _engine.GetStaticEvaluation() ?? throw new InvalidOperationException();
        _evaluationCollection.Add(_currentEvaluation);
    }

    private void UpdateGameData()
    {
        BestMovesCollection = new ObservableCollection<MoveEvaluation>(_engine.GetNBestMoves(10));
    }

    private void UpdateAnalysisData()
    {
        if (_currentEvaluation is not null)
            AnalysisReport = SimpleReport.GetSimpleReport(_currentEvaluation,
                _previousEvaluation,
                GameHandler.GetPlayerToPlay());
    }

    private void UpdateCurrentOpening()
    {
        var opening = OpeningFactory.GetOpening(GameHandler.GetCurrentFENPosition());
        CurrentOpening = $"Opening: {opening.Name}";
    }

    private ReactiveCommand<Unit, Unit> HandleFileExplorer()
    {
        return ReactiveCommand.CreateFromTask(async () =>
        {
            var parentWindow = new Window();
            var dialog = new OpenFileDialog
            {
                Title = "Select a file",
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Downloads"
            };
            dialog.Filters?.Add(new FileDialogFilter { Name = "PGN Files", Extensions = { "pgn" } });
            var result = await dialog.ShowAsync(parentWindow);
            if (result != null)
            {
                var selectedFilePath = result[0];
                HandleGameFileLoading(selectedFilePath);
            }
        });
    }

    private void HandleGameFileLoading(string filePath)
    {
        if (filePath != null)
        {
            var _newGameHandler = new GameHandler(_variant);
            _newGameHandler.LoadGame(filePath);
            GameHandler = _newGameHandler;
            UpdateEngine();
            UpdateGameData();
            UpdateAnalysisData();
            UpdateCurrentOpening();
        }
        else
        {
            throw new FileNotFoundException("Error! PGN file not found!");
        }
    }
}