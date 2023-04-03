using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ChessBoom.Models.Analysis;

public class Opening
{
    public string? Name { get; init; }
    public string? PGN { get; init; }
}

public static class OpeningFactory
{
    private static readonly Dictionary<string, Opening>? Openings;

    private static readonly string OpeningsPath =
        Path.Combine(System.AppContext.BaseDirectory, "Resources/openings.json");

    static OpeningFactory()
    {
        var openings = File.ReadAllText(OpeningsPath);
        Openings = JsonSerializer.Deserialize<Dictionary<string, Opening>>(openings);
    }

    public static Opening GetOpening(string fenPosition)
    {
        if (Openings != null && Openings.TryGetValue(fenPosition, out var opening)) return opening;

        return new Opening
        {
            Name = "Unknown",
            PGN = ""
        };
    }
}