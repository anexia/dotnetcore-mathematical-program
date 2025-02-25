// ------------------------------------------------------------------------------------------
//  <copyright file = "EnableSolverOutput.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// Represents a setting whether to enable solver log output on the console, or not.
/// </summary>
/// <param name="value">True to enable solver console log output, false to disable.</param>
public sealed class EnableSolverOutput(bool value) : MemberwiseEquatable<EnableSolverOutput>
{
    public static readonly EnableSolverOutput True = new(true);
    public static readonly EnableSolverOutput False = new(false);

    public bool Value { get; } = value;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{nameof(Value)}: {Value}";
}