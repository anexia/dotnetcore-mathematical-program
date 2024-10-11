// ------------------------------------------------------------------------------------------
//  <copyright file = "EnableSolverOutput.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.SolverConfiguration;

public sealed class EnableSolverOutput(bool value) : MemberwiseEquatable<EnableSolverOutput>
{
    public static readonly EnableSolverOutput True = new(true);
    public static readonly EnableSolverOutput False = new(false);

    public bool Value { get; } = value;

    public override string ToString() => $"{nameof(Value)}: {Value}";
}