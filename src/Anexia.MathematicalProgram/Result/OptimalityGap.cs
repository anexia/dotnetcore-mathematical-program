// ------------------------------------------------------------------------------------------
//  <copyright file = "OptimalityGap.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Represents an optimality gap.
/// </summary>
/// <param name="value">The gap's value.</param>
public sealed class OptimalityGap(double value) : MemberwiseEquatable<OptimalityGap>
{
    /// <summary>
    /// The optimality gap.
    /// </summary>
    public double Value { get; } = value;

    /// <inheritdoc />
    public override string ToString() => $"{Value}";
}