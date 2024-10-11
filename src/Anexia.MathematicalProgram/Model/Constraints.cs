// ------------------------------------------------------------------------------------------
//  <copyright file = "Constraints.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using System.Collections;
using System.Collections.Immutable;

#endregion

namespace Anexia.MathematicalProgram.Model;

public sealed class Constraints(ImmutableList<Constraint> elements) : IEnumerable<Constraint>, IEquatable<Constraints>
{
    private ImmutableList<Constraint> Elements { get; } = elements;

    public Constraints(IEnumerable<Constraint> elements)
        : this(elements.ToImmutableList())
    { }

    public Constraints(params Constraint[] values)
        : this(values.ToImmutableList())
    { }

    public Constraints()
        : this(ImmutableList<Constraint>.Empty)
    { }

    public bool Equals(Constraints? other)
    {
        if(other is null) return false;

        return ReferenceEquals(this, other) || Elements.SequenceEqual(other.Elements);
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Constraints other && Equals(other);

    public override int GetHashCode() => Elements.GetHashCode();

    public Constraints Add(Constraint constraint) => new(Elements.Add(constraint));

    public IEnumerator<Constraint> GetEnumerator() => Elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => string.Join(",", Elements);
}