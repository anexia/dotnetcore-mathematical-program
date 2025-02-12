// ------------------------------------------------------------------------------------------
//  <copyright file = "Variables.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents a collection of variables.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TVariableInterval">The type of the variable interval's scalar.</typeparam>
public readonly record struct Variables<TVariable, TVariableInterval> :
    IVariables<TVariable, TVariableInterval> where TVariable : IVariable<TVariableInterval>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
    private readonly ImmutableHashSet<TVariable> _elements;

    public Variables() : this([])
    {
    }

    public Variables(ImmutableHashSet<TVariable> elements)
    {
        _elements = elements;
    }

    /// <summary>
    /// The number of variables in the collection.
    /// </summary>
    public int Count => _elements.Count;

    public Variables<TVariable, TVariableInterval> Add(IVariable<TVariableInterval> variable) =>
        new(_elements.Add((TVariable)variable));

    /// <inheritdoc />
    public IEnumerator<TVariable> GetEnumerator() => _elements.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => string.Join(",", _elements);
}