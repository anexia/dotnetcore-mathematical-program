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

/// <summary>
/// Represents an immutable list of constraints.
/// </summary>
/// <param name="elements">The constraints.</param>
public sealed class Constraints(ImmutableList<Constraint> elements) : IEnumerable<Constraint>, IEquatable<Constraints>
{
    private ImmutableList<Constraint> Elements { get; } = elements;

    public Constraints(IEnumerable<Constraint> elements)
        : this(elements.ToImmutableList())
    {
    }

    public Constraints(params Constraint[] values)
        : this(values.ToImmutableList())
    {
    }

    public Constraints()
        : this(ImmutableList<Constraint>.Empty)
    {
    }


    /// <inheritdoc/>
    public bool Equals(Constraints? other)
    {
        if (other is null) return false;

        return ReferenceEquals(this, other) || Elements.SequenceEqual(other.Elements);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Constraints other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Elements.GetHashCode();

    /// <summary>
    /// Adds the given constraint.
    /// </summary>
    /// <param name="constraint">The constraint to be added.</param>
    /// <returns>A new object with the constraint added.</returns>
    public Constraints Add(Constraint constraint) => new(Elements.Add(constraint));

    /// <inheritdoc/>
    public IEnumerator<Constraint> GetEnumerator() => Elements.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => string.Join(",", Elements);
}