// ------------------------------------------------------------------------------------------
//  <copyright file = "WarmStart.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Immutable start solution that is passed to the solver as warm start / MIP start.
/// Use <see cref="Add"/> to create a new warm start containing an additional start value.
/// </summary>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
/// <typeparam name="TVariableInterval">The type of the variable interval's scalar, which is also the type of the start values.</typeparam>
public sealed class WarmStart<TVariable, TVariableInterval> : IWarmStart<TVariable, TVariableInterval>,
    IEquatable<WarmStart<TVariable, TVariableInterval>>
    where TVariable : IVariable<TVariableInterval>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
    private readonly ImmutableDictionary<TVariable, TVariableInterval> _startValues;

    /// <summary>
    /// Creates an empty warm start.
    /// </summary>
    public WarmStart() : this(ImmutableDictionary<TVariable, TVariableInterval>.Empty)
    {
    }

    /// <summary>
    /// Creates a warm start from the given variable-value pairs. When a variable occurs more than once,
    /// the last value wins.
    /// </summary>
    /// <param name="startValues">The start values.</param>
    public WarmStart(IEnumerable<KeyValuePair<TVariable, TVariableInterval>> startValues)
        : this(startValues.Aggregate(ImmutableDictionary<TVariable, TVariableInterval>.Empty,
            (current, pair) => current.SetItem(pair.Key, pair.Value)))
    {
    }

    private WarmStart(ImmutableDictionary<TVariable, TVariableInterval> startValues)
    {
        _startValues = startValues;
    }

    /// <summary>
    /// The number of variables having a start value.
    /// </summary>
    public int Count => _startValues.Count;

    /// <summary>
    /// True, when no start values are defined, false otherwise.
    /// </summary>
    public bool Empty => _startValues.Count == 0;

    /// <summary>
    /// Returns a new warm start that additionally contains the given start value. When the variable already has a
    /// start value, it is replaced.
    /// </summary>
    /// <param name="variable">The variable.</param>
    /// <param name="value">The value the variable should take in the start solution.</param>
    /// <returns>A new warm start including the given start value.</returns>
    public WarmStart<TVariable, TVariableInterval> Add(TVariable variable, TVariableInterval value) =>
        new(_startValues.SetItem(variable, value));

    /// <summary>
    /// Attempts to retrieve the start value of the given variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    /// <param name="value">The start value when present, null otherwise.</param>
    /// <returns>True, when the variable has a start value, false otherwise.</returns>
    public bool TryGetStartValue(TVariable variable, [NotNullWhen(true)] out TVariableInterval? value) =>
        _startValues.TryGetValue(variable, out value);

    /// <inheritdoc />
    public IEnumerator<IStartValue<TVariable, TVariableInterval>> GetEnumerator() =>
        _startValues.Select(pair => new StartValue<TVariable, TVariableInterval>(pair.Key, pair.Value))
            .GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool Equals(WarmStart<TVariable, TVariableInterval>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _startValues.Count == other._startValues.Count &&
               _startValues.All(pair =>
                   other._startValues.TryGetValue(pair.Key, out var otherValue) &&
                   EqualityComparer<TVariableInterval>.Default.Equals(pair.Value, otherValue));
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as WarmStart<TVariable, TVariableInterval>);

    /// <inheritdoc />
    public override int GetHashCode() =>
        _startValues.Select(pair => HashCode.Combine(pair.Key, pair.Value))
            .Order()
            .Aggregate(0, HashCode.Combine);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => string.Join(", ", this);
}
