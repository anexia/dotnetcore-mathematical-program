// ------------------------------------------------------------------------------------------
//  <copyright file = "RealInterval.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Interval;

/// <summary>
/// Represents an interval with real bounds.
/// </summary>
public sealed class RealInterval : MemberwiseEquatable<RealInterval>, IInterval<RealScalar>
{
    /// <summary>
    /// Represents an interval in the real number space defined by a lower and an upper bound.
    /// </summary>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <exception cref="InadmissibleBoundsException{RealScalar}">Thrown when lb > ub.</exception>
    public RealInterval(RealScalar lowerBound, RealScalar upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
        if (LowerBound.Value > UpperBound.Value)
            throw new InadmissibleBoundsException<RealScalar>(LowerBound, UpperBound);
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
    public override string ToString() => $"{nameof(LowerBound)}: {LowerBound}, {nameof(UpperBound)}: {UpperBound}";
}