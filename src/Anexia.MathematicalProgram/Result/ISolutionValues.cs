// ------------------------------------------------------------------------------------------
//  <copyright file = "ISolutionValues.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Holds information of the variable's value that occur in the solution.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TScalar">The scalar type of the variable's value.</typeparam>
/// <typeparam name="TVariableInterval">The type of the variable interval's scalar.</typeparam>
public interface
    ISolutionValues<in TVariable, TScalar, in TVariableInterval>
    where TVariable : IVariable<TVariableInterval>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
    /// <summary>
    /// Attempts to retrieve the solution value associated with the given variable.
    /// </summary>
    /// <param name="variable">The variable whose value in the solution is to be retrieved.</param>
    /// <returns>Returns the scalar variable has an associated value in the solution; otherwise, <c>null</c>.</returns>
    public TScalar? GetSolutionValueOrDefault(TVariable variable);

    public bool TryGetSolutionValue(TVariable variable, [NotNullWhen(true)] out TScalar? value);

    /// <summary>
    /// True if there are no variables, false otherwise.
    /// </summary>
    public bool Empty { get; }
}