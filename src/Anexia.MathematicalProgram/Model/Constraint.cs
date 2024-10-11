// ------------------------------------------------------------------------------------------
//  <copyright file = "Constraint.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

public sealed class Constraint(Terms terms, IInterval interval) : MemberwiseEquatable<Constraint>
{
    public Terms Terms { get; } = terms;
    public IInterval Interval { get; } = interval;

    public override string ToString() => $"{nameof(Interval)}: {Interval}, {nameof(Terms)}: {Terms}";
}