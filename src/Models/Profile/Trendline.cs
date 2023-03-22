using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessBoom.Models.Profile;

public class Trendline
{
    private readonly IList<int> _xAxisValues;
    private readonly IList<int> _yAxisValues;
    private int _count;
    private int _xAxisValuesSum;
    private int _xxSum;
    private int _xySum;
    private int _yAxisValuesSum;

    public Trendline(IList<int> yAxisValues, IList<int> xAxisValues)
    {
        _yAxisValues = yAxisValues;
        _xAxisValues = xAxisValues;

        Initialize();
    }

    public double Slope { get; private set; }
    public double Intercept { get; private set; }
    public int Start { get; private set; }
    public int End { get; private set; }

    private void Initialize()
    {
        _count = _yAxisValues.Count;
        _yAxisValuesSum = _yAxisValues.Sum();
        _xAxisValuesSum = _xAxisValues.Sum();
        _xxSum = 0;
        _xySum = 0;

        for (var i = 0; i < _count; i++)
        {
            _xySum += _xAxisValues[i] * _yAxisValues[i];
            _xxSum += _xAxisValues[i] * _xAxisValues[i];
        }

        Slope = CalculateSlope();
        Intercept = CalculateIntercept();
        Start = CalculateStart();
        End = CalculateEnd();
    }

    private double CalculateSlope()
    {
        try
        {
            return ((double)(_count * _xySum) - _xAxisValuesSum * _yAxisValuesSum) /
                   (_count * _xxSum - _xAxisValuesSum * _xAxisValuesSum);
        }
        catch (DivideByZeroException)
        {
            return 0;
        }
    }

    private double CalculateIntercept()
    {
        return (_yAxisValuesSum - Slope * _xAxisValuesSum) / _count;
    }

    private int CalculateStart()
    {
        return (int)(Slope * _xAxisValues.First() + Intercept);
    }

    private int CalculateEnd()
    {
        return (int)(Slope * _xAxisValues.Last() + Intercept);
    }
}