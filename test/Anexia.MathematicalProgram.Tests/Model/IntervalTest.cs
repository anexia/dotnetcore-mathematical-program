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
    [Theory]
    [InlineData(6, 5)]
    [InlineData(0.1, 0)]
    [InlineData(-0.1, -0.11)]
    [InlineData(double.MaxValue, double.Epsilon)]
    public void IntervalInitializingThrowsExpectedException(double left, double right) =>
        Assert.Throws<InadmissibleBoundsException<RealScalar>>(() =>
            new RealInterval(new RealScalar(left), new RealScalar(right)));
}