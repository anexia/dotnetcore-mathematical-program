// ------------------------------------------------------------------------------------------
//  <copyright file = "IntervalFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;

#endregion

namespace Anexia.MathematicalProgram.Tests.Factory;

internal static class IntervalFactory
{
    public static Interval Interval(double left, double right) => new(new LowerBound(left), new UpperBound(right));

    public static Point Point(double point) => new(point);
}