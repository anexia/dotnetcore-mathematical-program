// ------------------------------------------------------------------------------------------
//  <copyright file = "IntervalTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class IntervalTest
{
    [Fact]
    public void RealIntervalKeepsInclusiveBounds()
    {
        var interval = new RealInterval(new RealScalar(-1.5), new RealScalar(3.25));

        Assert.Equal(new RealScalar(-1.5), interval.LowerBound);
        Assert.Equal(new RealScalar(3.25), interval.UpperBound);
    }

    [Theory]
    [InlineData(6, 5)]
    [InlineData(0.1, 0)]
    [InlineData(-0.1, -0.11)]
    [InlineData(double.MaxValue, double.Epsilon)]
    public void IntervalInitializingThrowsExpectedException(double left, double right) =>
        Assert.Throws<InadmissibleBoundsException<RealScalar>>(() =>
            new RealInterval(new RealScalar(left), new RealScalar(right)));

    [Fact]
    public void IntegralIntervalKeepsInclusiveBounds()
    {
        var interval = new IntegralInterval(new IntegerScalar(-3), new IntegerScalar(7));

        Assert.Equal(new IntegerScalar(-3), interval.LowerBound);
        Assert.Equal(new IntegerScalar(7), interval.UpperBound);
    }

    [Theory]
    [InlineData(6, 5)]
    [InlineData(-1, -21)]
    [InlineData(int.MaxValue, 0)]
    public void IntegralIntervalInitializingThrowsExpectedException(int left, int right) =>
        Assert.Throws<InadmissibleBoundsException<IntegerScalar>>(() =>
            new IntegralInterval(new IntegerScalar(left), new IntegerScalar(right)));

    [Fact]
    public void BinaryIntervalAlwaysUsesZeroAndOneBounds()
    {
        var interval = new BinaryInterval();

        Assert.Equal(BinaryScalar.Zero, interval.LowerBound);
        Assert.Equal(BinaryScalar.One, interval.UpperBound);
    }

    [Theory]
    [InlineData(-2.5)]
    [InlineData(0)]
    [InlineData(4.75)]
    public void PointUsesSameValueForLowerAndUpperBounds(double value)
    {
        var point = new Point(value);

        Assert.Equal(new RealScalar(value), point.LowerBound);
        Assert.Equal(new RealScalar(value), point.UpperBound);
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(4)]
    public void IntegralPointUsesSameValueForLowerAndUpperBounds(int value)
    {
        var point = new IntegralPoint(value);

        Assert.Equal(new IntegerScalar(value), point.LowerBound);
        Assert.Equal(new IntegerScalar(value), point.UpperBound);
    }
}
