using ChessBoom.Models.Game;

namespace ChessBoom.Models.Analysis;

public class SimpleReport
{
    public string? Accuracy { get; set; }
    public string? Blunders { get; set; }
    public string? Risk { get; set; }
    public string? MissedWins { get; set; }
    
    public SimpleReport()
    {
        Accuracy = "Accuracy:";
        Blunders = "Blunder?:";
        Risk = "Risk:";
        MissedWins = "Missed Win?:";
    }

    public static SimpleReport GetSimpleReport(Evaluation current, Evaluation? previous, Player player)
    {
        var difference = current.FinalEvaluation;
        if (previous is not null)
        {
            if (player == Player.Black)
            {
                difference = -1 * current.FinalEvaluation + -1 * previous.FinalEvaluation;
            }
            else
            {
                difference = current.FinalEvaluation - previous.FinalEvaluation;
            }
        }

        var accuracy = "Brilliant Move";
        var risk = "None";
        var isBlunder = "No";
        var isMissedWin = "No";

        switch (difference)
        {
            case < -3:
                accuracy = "Blunder";
                risk = "Very High";
                isBlunder = "Yes";
                isMissedWin = "Yes";
                break;
            case < -2:
                accuracy = "Mistake";
                risk = "High";
                isBlunder = "Yes";
                break;
            case < -0.5f:
                accuracy = "Inaccuracy";
                risk = "Moderate";
                break;
            case < 0.5f:
                accuracy = "Accurate";
                risk = "Moderate";
                break;
            case < 2:
                accuracy = "Good Move";
                risk = "Low";
                break;
        }

        return new SimpleReport
        {
            Accuracy = $"Accuracy: {accuracy}",
            Blunders = $"Blunder?: {isBlunder}",
            Risk = $"Risk: {risk}",
            MissedWins = $"Missed Win?: {isMissedWin}"
        };
    }
}