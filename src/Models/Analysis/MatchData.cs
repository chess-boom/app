namespace ChessBoom.Models.Analysis;

public class MatchData
{
    public string? Name { get; set; }
    public string? Opponent { get; set; }
    public string? Variant { get; set; }
    public string? MatchStatus { get; set; }
    public int? Moves { get; set; }
    public int? Blunders { get; set; }
    public int? MissedWins { get; set; }
    public string? Improvement { get; set; }
}