// ------------------------------------------------------------------------------------------
//  <copyright file = "Constant.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


namespace Anexia.MathematicalProgram.Model;

public sealed class Constant(double value) : MemberwiseEquatable<Constant>
{
    public static readonly Constant Zero = new(0);

    public double Value { get; } = value;

    public Constant Plus(double value) => new(Value + value);

    public override string ToString() => $"{Value:F1}";
}