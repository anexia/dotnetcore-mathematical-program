// ------------------------------------------------------------------------------------------
//  <copyright file = "IsOptimal.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Represents an object containing optimality information.
/// </summary>
/// <param name="value">Boolean representing optimal when true, or not optimal when false.</param>
public sealed class IsOptimal(bool value) : MemberwiseEquatable<IsOptimal>
{
    /// <summary>
    /// True for optimal, false when not.
    /// </summary>
    public bool Value { get; } = value;

    /// <inheritdoc />
    public override string ToString() => $"{Value}";
}