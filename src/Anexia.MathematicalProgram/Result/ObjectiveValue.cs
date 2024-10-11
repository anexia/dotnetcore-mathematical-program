// ------------------------------------------------------------------------------------------
//  <copyright file = "ObjectiveValue.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Represents an objective value.
/// </summary>
/// <param name="value">The objective's value.</param>
public sealed class ObjectiveValue(double value) : MemberwiseEquatable<ObjectiveValue>
{
    /// <summary>
    /// The objective value.
    /// </summary>
    public double Value { get; } = value;

    /// <inheritdoc />
    public override string ToString() => $"{Value}";
}