// ------------------------------------------------------------------------------------------
//  <copyright file = "Point.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Interval;

/// <summary>
/// Represents an inclusive interval where the lower bound equals the upper bound.
/// </summary>
public readonly record struct Point : IInterval<RealScalar>
{
    /// <summary>
    /// Represents an interval [1,1]
    /// </summary>
    public static readonly Point One = new(1);

    public Point(double value)
        : this(new RealScalar(value),
            new RealScalar(value))
    {
    }

    private Point(RealScalar lowerBound, RealScalar upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    /// <summary>
    /// The lower bound.
    /// </summary>
    public RealScalar LowerBound { get; }

    /// <summary>
    /// The upper bound.
    /// </summary>
    public RealScalar UpperBound { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Point [{LowerBound},{UpperBound}]";
}