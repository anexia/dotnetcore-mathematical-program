// ------------------------------------------------------------------------------------------
//  <copyright file = "IsFeasible.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Result;

public sealed class IsFeasible(bool value) : MemberwiseEquatable<IsFeasible>
{
    public bool Value { get; } = value;

    public override string ToString() => $"{Value}";
}