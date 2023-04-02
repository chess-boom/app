namespace ChessBoom.Models.Analysis;

public class MoveEvaluation
{
    public string Move { get; }
    public int Evaluation { get; }
    
    public MoveEvaluation(string move, int evaluation)
    {
        Move = move;
        Evaluation = evaluation;
    }
}