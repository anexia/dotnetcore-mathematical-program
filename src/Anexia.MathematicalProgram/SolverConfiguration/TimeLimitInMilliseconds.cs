// ------------------------------------------------------------------------------------------
//  <copyright file = "TimeLimitInMilliseconds.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.SolverConfiguration;

public sealed class TimeLimitInMilliseconds(uint value) : MemberwiseEquatable<TimeLimitInMilliseconds>
{
    public static readonly TimeLimitInMilliseconds Unbounded = new(uint.MaxValue);

    public uint Value { get; } = value;

    public override string ToString() => $"{nameof(Value)}: {Value}";
}