﻿// ------------------------------------------------------------------------------------------
//  <copyright file = "OptimalityGap.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

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
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Value}";
}