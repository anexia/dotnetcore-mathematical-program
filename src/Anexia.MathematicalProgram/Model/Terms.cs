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

/// <summary>
/// Represents a set of terms.
/// </summary>
/// <param name="elements">A set of terms.</param>
public sealed class Terms(ImmutableHashSet<Term> elements) : IEquatable<Terms>, IEnumerable<Term>
{
    /// <summary>
    /// Initializes a new instance of <see cref="Terms"/> with given terms.
    /// </summary>
    /// <param name="elements">An enumerable of terms.</param>
    public Terms(IEnumerable<Term> elements)
        : this(elements.ToImmutableHashSet())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Terms"/> with given terms.
    /// </summary>
    /// <param name="values">A params list of terms.</param>
    public Terms(params Term[] values)
        : this(values.ToImmutableList())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Terms"/> with empty terms.
    /// </summary>
    public Terms()
        : this(ImmutableHashSet<Term>.Empty)
    {
    }

    /// <inheritdoc />
    public bool Equals(Terms? other)
    {
        if (other is null) return false;

        return ReferenceEquals(this, other) || Elements.SequenceEqual(other.Elements);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Terms other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Elements.GetHashCode();

    private ImmutableHashSet<Term> Elements { get; } = elements;

    /// <summary>
    /// Adds the given term to the end of the immutable list.
    /// </summary>
    /// <param name="term">The term to be added.</param>
    /// <returns>A new object with the term added.</returns>
    public Terms Add(Term term) => new(Elements.Add(term));

    /// <inheritdoc />
    public IEnumerator<Term> GetEnumerator() => Elements.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => string.Join(",", Elements);
}