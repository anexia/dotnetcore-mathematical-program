// ------------------------------------------------------------------------------------------
//  <copyright file = "IntegralPoint.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Interval;

/// <summary>
/// Represents an inclusive interval where the lower bound equals the upper bound.
/// </summary>
public readonly record struct IntegralPoint : IInterval<IntegerScalar>
{
    /// <summary>
    /// Represents an interval [1,1]
    /// </summary>
    public static readonly IntegralPoint One = new(1);

    public IntegralPoint(int value)
        : this(new IntegerScalar(value),
            new IntegerScalar(value))
    {
    }

    private IntegralPoint(IntegerScalar lowerBound, IntegerScalar upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
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
    public override string ToString() => $"IntegralPoint [{LowerBound},{UpperBound}]";
}