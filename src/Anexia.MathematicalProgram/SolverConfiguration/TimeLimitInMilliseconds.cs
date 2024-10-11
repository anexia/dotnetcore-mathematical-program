// ------------------------------------------------------------------------------------------
//  <copyright file = "TimeLimitInMilliseconds.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// The time limit.
/// </summary>
/// <param name="value"></param>
public sealed class TimeLimitInMilliseconds(uint value) : MemberwiseEquatable<TimeLimitInMilliseconds>
{
    public uint Value { get; } = value;

    /// <inderitdoc />
    public override string ToString() => $"{nameof(Value)}: {Value}";
}