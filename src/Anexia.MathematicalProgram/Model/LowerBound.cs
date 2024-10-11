// ------------------------------------------------------------------------------------------
//  <copyright file = "LowerBound.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents a lower bound.
/// </summary>
/// <param name="value">The lower bound's value.</param>
public sealed class LowerBound(double value) : MemberwiseEquatable<LowerBound>
{
    public double Value { get; } = value;

    /// <summary>
    /// Checks if a given lower bound is less than or equal to an upper bound.
    /// </summary>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    /// <returns>True, when the given lower bound is less than or equal to the given upper bound. False, otherwise.</returns>
    public static bool operator <=(LowerBound lowerBound, UpperBound upperBound) =>
        lowerBound.Value <= upperBound.Value;

    /// <summary>
    /// Checks if a given lower bound is greater than or equal to an upper bound.
    /// </summary>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    /// <returns>True, when the given lower bound is grater than or equal to the given upper bound. False, otherwise.</returns>
    public static bool operator >=(LowerBound lowerBound, UpperBound upperBound) =>
        lowerBound.Value >= upperBound.Value;

    /// <summary>
    /// Checks if a given lower bound is less than an upper bound.
    /// </summary>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    /// <returns>True, when the given lower bound is less than to the given upper bound. False, otherwise.</returns>
    public static bool operator <(LowerBound lowerBound, UpperBound upperBound) => lowerBound.Value < upperBound.Value;

    /// <summary>
    /// Checks if a given lower bound is greater than an upper bound.
    /// </summary>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    /// <returns>True, when the given lower bound is grater than the given upper bound. False, otherwise.</returns>
    public static bool operator >(LowerBound lowerBound, UpperBound upperBound) => lowerBound.Value > upperBound.Value;

    /// <inheritdoc/>
    public override string ToString() => $"{Value:F1}";
}