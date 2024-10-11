// ------------------------------------------------------------------------------------------
//  <copyright file = "Coefficient.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

public sealed class Coefficient(double value) : MemberwiseEquatable<Coefficient>
{
    public double Value { get; } = value;

    public override string ToString() => $"{Value:F1}";
}