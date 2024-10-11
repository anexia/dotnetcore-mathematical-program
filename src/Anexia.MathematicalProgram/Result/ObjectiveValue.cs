// ------------------------------------------------------------------------------------------
//  <copyright file = "ObjectiveValue.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Result;

public sealed class ObjectiveValue(double value) : MemberwiseEquatable<ObjectiveValue>
{
    public double Value { get; } = value;

    public override string ToString() => $"{Value}";
}