using System;

namespace ChessBoom.Models.Analysis;

/// <summary>
/// The StockfishReadyException class is used for when the Engine is not ready to receive commands.
/// </summary>
public class StockfishReadyException : Exception
{
    public StockfishReadyException()
    {
    }

    public StockfishReadyException(string message) : base(message)
    {
    }

    public StockfishReadyException(string message, Exception inner) : base(message, inner)
    {
    }
}