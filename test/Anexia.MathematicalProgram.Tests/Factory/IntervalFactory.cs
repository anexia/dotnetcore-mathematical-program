// ------------------------------------------------------------------------------------------
//  <copyright file = "IntervalFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Tests.Factory;

internal static class IntervalFactory
{
    public static RealInterval Interval(double left, double right) => new(
        new RealScalar(left),
        new RealScalar(right));

    public static IntegralInterval Interval(long left, long right) => new(
        new IntegerScalar(left),
        new IntegerScalar(right));

    public static Point Point(double point) => new(point);

    public static IntegralPoint Point(int point) => new(point);
}