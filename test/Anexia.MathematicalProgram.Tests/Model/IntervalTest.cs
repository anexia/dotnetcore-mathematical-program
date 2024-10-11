// ------------------------------------------------------------------------------------------
//  <copyright file = "IntervalTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;

#endregion

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class IntervalTest
{
    [Theory]
    [InlineData(6, 5)]
    [InlineData(0.1, 0)]
    [InlineData(-0.1, -0.11)]
    [InlineData(double.MaxValue, double.Epsilon)]
    public void IntervalInitializingThrowsExpectedException(double left, double right) =>
        Assert.Throws<InadmissibleBoundsException>(() => new Interval(new LowerBound(left), new UpperBound(right)));

    [Fact]
    public void SuccessfulIntervalInitializing() => Assert.NotNull(new Interval(new LowerBound(5), new UpperBound(6)));
}