// ------------------------------------------------------------------------------------------
//  <copyright file = "NumberOfThreads.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// Represents the number of threads to be used by the solver.
/// </summary>
/// <param name="value">The value.</param>
public sealed class NumberOfThreads(uint value) : MemberwiseEquatable<NumberOfThreads>
{
    public uint Value { get; } = value;

    /// <inderitdoc />
    public override string ToString() => $"{nameof(Value)}: {Value}";
}