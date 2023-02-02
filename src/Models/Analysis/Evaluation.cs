namespace ChessBoom.Models.Analysis;

/// <summary>
/// Evaluation class. Contains easily accessible information parsed from the Analysis engine.
/// </summary>
public class Evaluation
{
    /// <summary>
    /// Final evaluation value for the position
    /// </summary>
    public float FinalEvaluation { get; }

    /// <summary>
    /// Side of evaluation (w for white, b for black)
    /// </summary>
    public char Side { get; }

    /// <summary>
    /// Collection of evaluation information for the position.
    /// </summary>
    /// <param name="finalEvaluation">Final evaluation for the position</param>
    /// <param name="side">Side evaluation is for</param>
    public Evaluation(float finalEvaluation, char side)
    {
        FinalEvaluation = finalEvaluation;
        Side = side;
    }

    public override string ToString()
    {
        var sideString = (Side) == 'w' ? "White" : "Black";
        return $"Final Evaluation: {FinalEvaluation} for {sideString}";
    }
}
