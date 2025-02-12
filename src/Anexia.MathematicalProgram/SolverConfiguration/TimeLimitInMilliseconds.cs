// ------------------------------------------------------------------------------------------
//  <copyright file = "TimeLimitInMilliseconds.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// The time limit.
/// </summary>
/// <param name="value"></param>
public sealed class TimeLimitInMilliseconds(uint value) : MemberwiseEquatable<TimeLimitInMilliseconds>
{
    public uint Value { get; } = value;
    public uint AsSeconds => Value / 1000;

    /// <inderitdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{nameof(Value)}: {Value}";
}