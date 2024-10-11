// ------------------------------------------------------------------------------------------
//  <copyright file = "RelativeGap.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.SolverConfiguration;

public sealed class RelativeGap(double relativeGap) : MemberwiseEquatable<RelativeGap>
{
    public static readonly RelativeGap EMinus7 = new(0.0000001);

    public double Value { get; } = relativeGap;

    public static RelativeGap FromEMinus(uint negativeExponent) => new(Math.Pow(10, -negativeExponent));

    public override string ToString() => $"{nameof(Value)}: {Value}";
}