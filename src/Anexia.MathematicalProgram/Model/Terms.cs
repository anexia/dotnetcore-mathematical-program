// ------------------------------------------------------------------------------------------
//  <copyright file = "Terms.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using System.Collections;
using System.Collections.Immutable;

#endregion

namespace Anexia.MathematicalProgram.Model;

public sealed class Terms(ImmutableHashSet<Term> elements) : IEquatable<Terms>, IEnumerable<Term>
{
    public Terms(IEnumerable<Term> elements)
        : this(elements.ToImmutableHashSet())
    { }

    public Terms(params Term[] values)
        : this(values.ToImmutableList())
    { }

    public Terms()
        : this(ImmutableHashSet<Term>.Empty)
    { }

    public bool Equals(Terms? other)
    {
        if(other is null) return false;

        return ReferenceEquals(this, other) || Elements.SequenceEqual(other.Elements);
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Terms other && Equals(other);

    public override int GetHashCode() => Elements.GetHashCode();

    private ImmutableHashSet<Term> Elements { get; } = elements;

    public Terms Add(Term term) => new(Elements.Add(term));

    public IEnumerator<Term> GetEnumerator() => Elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => string.Join(",", Elements);
}