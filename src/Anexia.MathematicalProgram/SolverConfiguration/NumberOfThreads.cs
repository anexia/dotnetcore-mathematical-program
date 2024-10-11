// ------------------------------------------------------------------------------------------
//  <copyright file = "NumberOfThreads.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.SolverConfiguration;

public sealed class NumberOfThreads(uint value) : MemberwiseEquatable<NumberOfThreads>
{
    public uint Value { get; } = value;

    public override string ToString() => $"{nameof(Value)}: {Value}";
}