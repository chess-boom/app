using System.Collections.Generic;

namespace ChessBoom.Models.Analysis;

/// <summary>
/// Analysis interface. Common interface used to allow for future changes/expansions of engine options for analysis.
/// </summary>
public interface IAnalysis
{
    /// <summary>
    /// FEN position we wish to analyze. When setting the fen position, sends a command to set the position in the engine.
    /// </summary>
    string? FenPosition { get; set; }

    /// <summary>
    /// Variant of chess we are analyzing. When setting the variant, sends a command to set the variant in the engine.
    /// </summary>
    string? Variant { get; set; }

    /// <summary>
    /// Gets the static evaluation of the current position as a float, and which side the value is relative to (White or Black).
    /// </summary>
    /// <returns>Evaluation Object. Null if an evaluation cannot be obtained for some reason.</returns>
    Evaluation? GetStaticEvaluation();

    /// <summary>
    /// Returns the N best moves based on your position.
    /// </summary>
    /// <param name="n">Number of moves you want to return. Value greater than 0. Ordered by descending cp value). Default: 3</param>
    /// <param name="depth">Depth you want the search to go to. Value greater than 0. Default: 10</param>
    /// <returns>List of (string, int) tuples, representing (move, cp value). Ordered from highest cp to lowest cp value moves</returns>
    public List<(string, int)> GetNBestMoves(int n = 3, int depth = 10);

    /// <summary>
    /// Checks if the engine process is running (responding)
    /// </summary>
    /// <returns>Boolean for whether or not engine is running</returns>
    public bool IsRunning();

    /// <summary>
    /// Close the engine process.
    /// </summary>
    public void Close();
}
