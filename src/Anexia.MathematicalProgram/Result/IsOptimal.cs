// ------------------------------------------------------------------------------------------
//  <copyright file = "IsOptimal.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Result;

public sealed class IsOptimal(bool value) : MemberwiseEquatable<IsOptimal>
{
    public bool Value { get; } = value;

    public override string ToString() => $"{Value}";
}