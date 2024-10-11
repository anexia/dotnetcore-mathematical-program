// ------------------------------------------------------------------------------------------
//  <copyright file = "OptimalityGap.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Result;

public sealed class OptimalityGap(double value) : MemberwiseEquatable<OptimalityGap>
{
    public double Value { get; } = value;

    public override string ToString() => $"{Value}";
}