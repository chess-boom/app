using System;
using System.Collections.Generic;
using System.Linq;

public class Trendline
{
    private readonly IList<int> xAxisValues;
    private readonly IList<int> yAxisValues;
    private int count;
    private int xAxisValuesSum;
    private int xxSum;
    private int xySum;
    private int yAxisValuesSum;

    public Trendline(IList<int> yAxisValues, IList<int> xAxisValues)
    {
        this.yAxisValues = yAxisValues;
        this.xAxisValues = xAxisValues;

        this.Initialize();
    }

    public double Slope { get; private set; }
    public double Intercept { get; private set; }
    public int Start { get; private set; }
    public int End { get; private set; }

    private void Initialize()
    {
        this.count = this.yAxisValues.Count;
        this.yAxisValuesSum = this.yAxisValues.Sum();
        this.xAxisValuesSum = this.xAxisValues.Sum();
        this.xxSum = 0;
        this.xySum = 0;

        for (int i = 0; i < this.count; i++)
        {
            this.xySum += (this.xAxisValues[i]*this.yAxisValues[i]);
            this.xxSum += (this.xAxisValues[i]*this.xAxisValues[i]);
        }
        this.Slope = this.CalculateSlope();
        this.Intercept = this.CalculateIntercept();
        this.Start = this.CalculateStart();
        this.End = this.CalculateEnd();
    }

    private double CalculateSlope()
    {
        try
        {
            return ((double)(this.count*this.xySum) - (this.xAxisValuesSum*this.yAxisValuesSum))/((this.count*this.xxSum) - (this.xAxisValuesSum*this.xAxisValuesSum));
        }
        catch (DivideByZeroException)
        {
            return 0;
        }
    }

    private double CalculateIntercept()
    {
        return (this.yAxisValuesSum - (this.Slope*this.xAxisValuesSum))/this.count;
    }

    private int CalculateStart()
    {
        return (int)((this.Slope*this.xAxisValues.First()) + this.Intercept);
    }

    private int CalculateEnd()
    {
        return (int)((this.Slope*this.xAxisValues.Last()) + this.Intercept);
    }
}