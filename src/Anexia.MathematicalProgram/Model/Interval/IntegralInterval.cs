// ------------------------------------------------------------------------------------------
//  <copyright file = "IntegralInterval.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Interval;

/// <summary>
/// Represents an integral interval.
/// </summary>
public sealed class IntegralInterval : MemberwiseEquatable<IntegralInterval>, IInterval<IntegerScalar>
{
    /// <summary>
    /// Creates a new instance of <see cref="IntegralInterval"/> with given lower and upper bound.
    /// </summary>
    /// <param name="lowerBound">The interval's lower bound.</param>
    /// <param name="upperBound">The interval's upper bound.</param>
    /// <exception>Thrown when lb > ub.</exception>
    public IntegralInterval(IntegerScalar lowerBound, IntegerScalar upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
        if (LowerBound.Value > UpperBound.Value)
            throw new InadmissibleBoundsException<IntegerScalar>(LowerBound, UpperBound);
    }

    /// <summary>
    /// The lower bound.
    /// </summary>
    public IntegerScalar LowerBound { get; }

    /// <summary>
    /// The upper bound.
    /// </summary>
    public IntegerScalar UpperBound { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{nameof(LowerBound)}: {LowerBound}, {nameof(UpperBound)}: {UpperBound}";
}