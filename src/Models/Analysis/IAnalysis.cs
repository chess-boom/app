using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBoom.Models.Analysis
{
    /// <summary>
    /// Analysis interface. Common interface used to allow for future changes/expansions of engine options for analysis.
    /// </summary>
    internal interface IAnalysis
    {
        string FenPosition { get; set;}
        Evaluation GetStaticEvaluation();
    }
}
