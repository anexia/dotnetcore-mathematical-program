// ------------------------------------------------------------------------------------------
//  <copyright file = "WeightedSum.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Expression;

/// <summary>
/// Represents a sum of terms (Sum (coefficient * variable) ).
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the variable interval's scalar.</typeparam>
public sealed class
    WeightedSum<TVariable, TCoefficient, TInterval> :
    MemberwiseEquatable<WeightedSum<TVariable, TCoefficient, TInterval>>,
    IWeightedSum<TVariable, TCoefficient, TInterval>
    where TVariable : IVariable<TInterval>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TInterval : IAddableScalar<TInterval, TInterval>
{
    private readonly ImmutableDictionary<TVariable, TCoefficient> _elements;

    /// <summary>
    /// Initializes a new instance of <see cref="WeightedSum{TVariable,TCoefficient,TInterval}"/>.
    /// </summary>
    public WeightedSum()
        : this(ImmutableDictionary<TVariable, TCoefficient>.Empty)
    {
    }

    internal WeightedSum(ImmutableDictionary<TVariable, TCoefficient> elements)
    {
        _elements = elements;
    }

    /// <summary>
    /// Adds a new term represented by the given coefficient * variable to the sum.
    /// </summary>
    /// <param name="variable">The variable.</param>
    /// <param name="coefficient">The variable's coefficient.</param>
    /// <returns></returns>
    public WeightedSum<TVariable, TCoefficient, TInterval> Add(TVariable variable, TCoefficient coefficient) =>
        _elements.TryGetValue(variable, out var oldCoefficient)
            ? new WeightedSum<TVariable, TCoefficient, TInterval>(_elements.SetItem(variable,
                coefficient.Add(oldCoefficient)))
            : new WeightedSum<TVariable, TCoefficient, TInterval>(_elements.Add(variable, coefficient));

    /// <summary>
    /// Adds another sum to the sum.
    /// </summary>
    /// <param name="weightedSum">The sum to be added.</param>
    /// <returns>A new sum including the new sum.</returns>
    public WeightedSum<TVariable, TCoefficient, TInterval> Add(
        WeightedSum<TVariable, TCoefficient, TInterval> weightedSum) =>
        weightedSum._elements.Aggregate(this, (newSum, pair) =>
            newSum.Add(pair.Key, pair.Value));

    /// <inheritdoc />
    public IEnumerator<ITerm<TVariable, TCoefficient, TInterval>> GetEnumerator()
        => _elements.Select(item =>
            new Term<TVariable, TCoefficient, TInterval>(item.Value, item.Key) as
                ITerm<TVariable, TCoefficient, TInterval>).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => string.Join("", _elements);
}