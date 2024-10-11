// ------------------------------------------------------------------------------------------
//  <copyright file = "LowerBound.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

public sealed class LowerBound(double value) : MemberwiseEquatable<LowerBound>
{
    public double Value { get; } = value;

    public bool IsLessThanOrEqualTo(UpperBound upperBound) => Value <= upperBound.Value;

    public override string ToString() => $"{Value:F1}";
}