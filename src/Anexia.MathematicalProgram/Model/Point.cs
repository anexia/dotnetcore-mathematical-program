// ------------------------------------------------------------------------------------------
//  <copyright file = "Point.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents an inclusive interval where the lower bound equals the upper bound.
/// </summary>
public sealed class Point : MemberwiseEquatable<Point>, IInterval
{
    /// <summary>
    /// Represents an interval [1,1]
    /// </summary>
    public static readonly Point One = new(1);

    public Point(double value)
        : this(new LowerBound(value), new UpperBound(value))
    {
    }

    private Point(LowerBound lowerBound, UpperBound upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    /// <summary>
    /// The lower bound.
    /// </summary>
    public LowerBound LowerBound { get; }

    /// <summary>
    /// The upper bound.
    /// </summary>
    public UpperBound UpperBound { get; }

    /// <inheritdoc />
    public override string ToString() => $"Point [{LowerBound},{UpperBound}]";
}