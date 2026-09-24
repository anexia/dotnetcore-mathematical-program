// ------------------------------------------------------------------------------------------
//  <copyright file = "VariableAttributes.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Immutable collection of solver specific variable attributes, see <see cref="VariableAttributeType"/>.
/// Use <see cref="Add"/> to create a new collection containing an additional attribute.
/// </summary>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
public sealed class VariableAttributes<TVariable> : IVariableAttributes<TVariable>,
    IEquatable<VariableAttributes<TVariable>>
    where TVariable : notnull
{
    private readonly ImmutableDictionary<(TVariable Variable, VariableAttributeType Type), double> _attributes;

    /// <summary>
    /// Creates an empty collection of variable attributes.
    /// </summary>
    public VariableAttributes() : this(
        ImmutableDictionary<(TVariable Variable, VariableAttributeType Type), double>.Empty)
    {
    }

    private VariableAttributes(ImmutableDictionary<(TVariable Variable, VariableAttributeType Type), double> attributes)
    {
        _attributes = attributes;
    }

    /// <summary>
    /// The number of attributes.
    /// </summary>
    public int Count => _attributes.Count;

    /// <summary>
    /// True, when no attributes are defined, false otherwise.
    /// </summary>
    public bool Empty => _attributes.Count == 0;

    /// <summary>
    /// Returns a new collection that additionally contains the given attribute. When the variable already has an
    /// attribute of the same type, it is replaced.
    /// </summary>
    /// <param name="variable">The variable.</param>
    /// <param name="type">The type of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>A new collection including the given attribute.</returns>
    public VariableAttributes<TVariable> Add(TVariable variable, VariableAttributeType type, double value) =>
        new(_attributes.SetItem((variable, type), value));

    /// <summary>
    /// Attempts to retrieve the value of the given attribute of the given variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    /// <param name="type">The type of the attribute.</param>
    /// <param name="value">The attribute's value when present.</param>
    /// <returns>True, when the attribute is set for the variable, false otherwise.</returns>
    public bool TryGetValue(TVariable variable, VariableAttributeType type, out double value) =>
        _attributes.TryGetValue((variable, type), out value);

    /// <inheritdoc />
    public IEnumerator<IVariableAttribute<TVariable>> GetEnumerator() =>
        _attributes.Select(pair => new VariableAttribute<TVariable>(pair.Key.Variable, pair.Key.Type, pair.Value))
            .GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool Equals(VariableAttributes<TVariable>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _attributes.Count == other._attributes.Count &&
               _attributes.All(pair =>
                   other._attributes.TryGetValue(pair.Key, out var otherValue) && pair.Value.Equals(otherValue));
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as VariableAttributes<TVariable>);

    /// <inheritdoc />
    public override int GetHashCode() =>
        _attributes.Select(pair => HashCode.Combine(pair.Key.Variable, pair.Key.Type, pair.Value))
            .Order()
            .Aggregate(0, HashCode.Combine);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => string.Join(", ", this);
}
