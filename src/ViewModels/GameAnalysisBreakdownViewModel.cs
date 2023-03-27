using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Analysis;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class GameAnalysisBreakdownViewModel : BoardViewModel
{
    private ObservableCollection<Evaluation> _evaluationCollection;

    public ObservableCollection<Evaluation> EvaluationCollection
    {
        get => _evaluationCollection;
        set => this.RaiseAndSetIfChanged(ref _evaluationCollection, value);
    }

    private Stockfish _engine;

    public GameAnalysisBreakdownViewModel(IScreen hostScreen) : base(hostScreen)
    {
        _engine = new Stockfish
        {
            FenPosition = GameHandler.GetCurrentFENPosition()
        };

        _evaluationCollection = new ObservableCollection<Evaluation>
        {
            _engine.GetStaticEvaluation()!
        };
        GameHandler.MovePlayed += UpdateEngine;
    }

    private void UpdateEngine(string startingSquare, string destinationSquare)
    {
        Debug.WriteLine($"Move played: {startingSquare} -> {destinationSquare}");

        Debug.WriteLine("Updating engine...");
        Debug.WriteLine($"FEN before: {_engine.FenPosition}");
        _engine.FenPosition = GameHandler.GetCurrentFENPosition();
        Debug.WriteLine($"FEN after: {_engine.FenPosition}");

        Debug.WriteLine("Updating evaluation collection...");
        foreach (var evaluation in EvaluationCollection)
        {
            Debug.WriteLine($"Evaluation before: {evaluation}");
        }

        _evaluationCollection.Add(_engine.GetStaticEvaluation()!);
        foreach (var evaluation in EvaluationCollection)
        {
            Debug.WriteLine($"Evaluation after: {evaluation}");
        }
    }
}