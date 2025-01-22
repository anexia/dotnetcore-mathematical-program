// ------------------------------------------------------------------------------------------
//  <copyright file = "Terms.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using Google.OrTools.ConstraintSolver;
using Google.OrTools.LinearSolver;

#endregion

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents a set of terms.
/// </summary>
/// <param name="elements">A dictionary of variable - coefficient pairs.</param>
public sealed class Terms(ImmutableDictionary<Variable, Coefficient> elements) : IEquatable<Terms>, IEnumerable<Term>
{
    /// <summary>
    /// Initializes a new instance of <see cref="Terms"/> with given terms.
    /// </summary>
    /// <param name="elements">An enumerable of terms.</param>
    public Terms(IEnumerable<Term> elements)
        : this(elements.GroupBy(term => term.Variable).ToImmutableDictionary(key => key.Key,
            value => new Coefficient(value.Sum(item => item.Coefficient.Value))))
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

        if (ReferenceEquals(this, other)) return true;
        if (Elements.Count != other.Count())
            return false;

        foreach (var (key, value) in Elements)
        {
            if (!other.Elements.TryGetValue(key, out var val) || !value.Equals(val)) return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Terms other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Elements.GetHashCode();

    private ImmutableDictionary<Variable, Coefficient> Elements { get; } = elements;

    /// <summary>
    /// Adds the given term to the dictionary. If the term's variable already exists, the new coefficient gets added to the old.
    /// </summary>
    /// <param name="term">The term to be added.</param>
    /// <returns>A new object with the term added.</returns>
    public Terms Add(Term term) =>
        Elements.TryGetValue(term.Variable, out var coefficient)
            ? new(Elements.SetItem(term.Variable, new(coefficient.Value + term.Coefficient.Value)))
            : new(Elements.Add(term.Variable, term.Coefficient));

    public IEnumerable<KeyValuePair<Variable, Coefficient>> KeyValuePairs => Elements;

    /// <inheritdoc />
    public IEnumerator<Term> GetEnumerator() => Elements.Select(pair => new Term(pair.Value, pair.Key)).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => string.Join(",", Elements);
}