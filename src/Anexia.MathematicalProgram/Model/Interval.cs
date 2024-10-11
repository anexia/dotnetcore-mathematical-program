// ------------------------------------------------------------------------------------------
//  <copyright file = "Interval.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents a simple bounded interval with inclusive upper and lower bounds.
/// </summary>
public sealed class Interval : MemberwiseEquatable<Interval>, IInterval
{
    /// <summary>
    /// An interval with bounds [0,1].
    /// </summary>
    public static readonly Interval BinaryInterval = new(0, 1);

    /// <summary>
    /// Initializes an instance of type <see cref="Interval"/> with the given bounds.
    /// </summary>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    /// <exception cref="InadmissibleBoundsException">Throws an <see cref="InadmissibleBoundsException"/> when the lower bound is grater than the upper bound.</exception>
    public Interval(LowerBound lowerBound, UpperBound upperBound)
    {
        if (lowerBound > upperBound) throw new InadmissibleBoundsException(lowerBound, upperBound);
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    internal Interval(double lowerBound, double upperBound)
        : this(new LowerBound(lowerBound), new UpperBound(upperBound))
    {
    }

    /// <summary>
    /// The lower bound.
    /// </summary>
    public LowerBound LowerBound { get; }

    /// <summary>
    /// The upper bound.
    /// </summary>
    public UpperBound UpperBound { get; }

    /// <inheritdoc/>
    public override string ToString() => $"Interval [{LowerBound},{UpperBound}]";
}