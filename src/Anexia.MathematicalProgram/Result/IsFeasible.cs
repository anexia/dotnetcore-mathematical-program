// ------------------------------------------------------------------------------------------
//  <copyright file = "IsFeasible.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Represents an object containing feasibility information.
/// </summary>
/// <param name="value">Boolean representing feasible when true, or unfeasible when false.</param>
public sealed class IsFeasible(bool value) : MemberwiseEquatable<IsFeasible>
{
    /// <summary>
    /// True for feasible, false when not.
    /// </summary>
    public bool Value { get; } = value;

    /// <inheritdoc />
    public override string ToString() => $"{Value}";
}