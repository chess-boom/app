using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ChessBoom.Models.Analysis
{
    public class Opening
    {
        public string? Name { get; init; }
        public string? PGN { get; init; }
    }

    public static class OpeningFactory
    {
        private static readonly Dictionary<string, Opening>? Openings =
            JsonSerializer.Deserialize<Dictionary<string, Opening>>(
                File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources/openings.json"))
            );

        public static Opening GetOpening(string fenPosition)
        {
            if (Openings != null && Openings.TryGetValue(fenPosition, out var opening))
                return opening;

            return new Opening
            {
                Name = "Unknown",
                PGN = ""
            };
        }
    }
}