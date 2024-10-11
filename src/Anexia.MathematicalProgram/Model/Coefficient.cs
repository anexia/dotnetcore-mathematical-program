// ------------------------------------------------------------------------------------------
//  <copyright file = "Coefficient.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents a coefficient.
/// </summary>
/// <param name="value">The coefficient's value.</param>
public sealed class Coefficient(double value) : MemberwiseEquatable<Coefficient>
{
    public double Value { get; } = value;

    /// <inheritdoc />
    public override string ToString() => $"{Value:F1}";
}