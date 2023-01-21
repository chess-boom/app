namespace ChessBoom.Models.Analysis;

/// <summary>
/// Evaluation class. Contains easily accessible information parsed from the Analysis engine.
/// </summary>
public class Evaluation
{

    protected float m_finalEvaluation;
    /// <summary>
    /// Which side the evaluation is for (w for white, b for black)
    /// </summary>
    protected char m_side;
    /// <summary>
    /// Final evaluation value for the position
    /// </summary>
    public float FinalEvaluation
    {
        get { return m_finalEvaluation; }
        set { m_finalEvaluation = value; }
    }
    /// <summary>
    /// Side of evaluation (w for white, b for black)
    /// </summary>
    public char Side
    {
        get { return m_side; }
        set { m_side = value; }
    }
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
        string side_string = (m_side) == 'w' ? "White" : "Black";
        return $"Final Evaluation: {m_finalEvaluation} for {side_string}";
    }

}
