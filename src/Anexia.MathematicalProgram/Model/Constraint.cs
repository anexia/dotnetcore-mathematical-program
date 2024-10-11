// ------------------------------------------------------------------------------------------
//  <copyright file = "Constraint.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents a constraint.
/// </summary>
/// <param name="terms">A list of terms.</param>
/// <param name="interval">The lower and upper bound for the sum of the terms.</param>
public sealed class Constraint(Terms terms, IInterval interval) : MemberwiseEquatable<Constraint>
{
    public Terms Terms { get; } = terms;
    public IInterval Interval { get; } = interval;

    /// <inheritdoc/>
    public override string ToString() => $"{nameof(Interval)}: {Interval}, {nameof(Terms)}: {Terms}";
}