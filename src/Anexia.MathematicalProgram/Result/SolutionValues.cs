// ------------------------------------------------------------------------------------------
//  <copyright file = "SolutionValues.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Holds information of the variable's value that occur in the solution.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TScalar">The scalar type of the variable's solution.</typeparam>
/// <typeparam name="TVariableInterval">The type of the variable interval's scalar.</typeparam>
public sealed class SolutionValues<TVariable, TScalar, TVariableInterval> :
    MemberwiseEquatable<SolutionValues<TVariable, TScalar, TVariableInterval>>,
    IEnumerable<KeyValuePair<TVariable, TScalar>>,
    ISolutionValues<TVariable, TScalar, TVariableInterval>
    where TVariable : IVariable<TVariableInterval>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
    public SolutionValues(IReadOnlyDictionary<TVariable, TScalar> variablesMapping)
    {
        _variablesMapping = variablesMapping;
    }

    private readonly IReadOnlyDictionary<TVariable, TScalar> _variablesMapping;

    /// <summary>
    /// True, when no variables are defined in the model, false otherwise.
    /// </summary>
    public bool Empty => _variablesMapping.Count == 0;

    /// <summary>
    /// Attempts to retrieve the solution value associated with the given variable.
    /// </summary>
    /// <param name="variable">The variable whose value in the solution is to be retrieved.</param>
    /// <returns>Returns the variable's value in the solution when present, otherwise, <c>null</c>.</returns>
    public TScalar? GetSolutionValueOrDefault(TVariable variable) =>
        _variablesMapping.GetValueOrDefault(variable);

    /// <summary>
    /// Attempts to retrieve the solution value associated with the given variable.
    /// </summary>
    /// <param name="variable">The variable whose value in the solution is to be retrieved.</param>
    /// <param name="value">The variable value when present in the solution, null when not available.</param>
    /// <returns>True, when the variable is present, null otherwise.</returns>
    public bool TryGetSolutionValue(TVariable variable, [NotNullWhen(true)] out TScalar? value) =>
        _variablesMapping.TryGetValue(variable, out value);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TVariable, TScalar>> GetEnumerator() => _variablesMapping.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => string.Join(", ", _variablesMapping.Select(x => $"{x.Key.Name}={x.Value}"));
}