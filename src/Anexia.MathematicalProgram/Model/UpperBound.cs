// ------------------------------------------------------------------------------------------
//  <copyright file = "UpperBound.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents an upper bound.
/// </summary>
/// <param name="value">The upper bound's value.</param>
public sealed class UpperBound(double value) : MemberwiseEquatable<UpperBound>
{
    public double Value { get; } = value;

    /// <summary>
    /// Checks if a given upper bound is less than or equal to a lower bound.
    /// </summary>
    /// <param name="upperBound">The upper bound.</param>
    /// <param name="lowerBound">The lower bound.</param>
    /// <returns>True, when the given upper bound is less than or equal to the given lower bound. False, otherwise.</returns>
    public static bool operator <=(UpperBound upperBound, LowerBound lowerBound) =>
        upperBound.Value <= lowerBound.Value;

    /// <summary>
    /// Checks if a given upper bound is greater than or equal to an lower bound.
    /// </summary>
    /// <param name="upperBound">The upper bound.</param>
    /// <param name="lowerBound">The lower bound.</param>
    /// <returns>True, when the given upper bound is grater than or equal to the given lower bound. False, otherwise.</returns>
    public static bool operator >=(UpperBound upperBound, LowerBound lowerBound) =>
        upperBound.Value >= lowerBound.Value;

    /// <summary>
    /// Checks if a given upper bound is less than an lower bound.
    /// </summary>
    /// <param name="upperBound">The upper bound.</param>
    /// <param name="lowerBound">The lower bound.</param>
    /// <returns>True, when the given upper bound is less than to the given lower bound. False, otherwise.</returns>
    public static bool operator <(UpperBound upperBound, LowerBound lowerBound) => upperBound.Value < lowerBound.Value;

    /// <summary>
    /// Checks if a given upper bound is greater than an lower bound.
    /// </summary>
    /// <param name="upperBound">The upper bound.</param>
    /// <param name="lowerBound">The lower bound.</param>
    /// <returns>True, when the given upper bound is grater than the given lower bound. False, otherwise.</returns>
    public static bool operator >(UpperBound upperBound, LowerBound lowerBound) => upperBound.Value > lowerBound.Value;

    /// <inheritdoc />
    public override string ToString() => $"{Value:F1}";
}