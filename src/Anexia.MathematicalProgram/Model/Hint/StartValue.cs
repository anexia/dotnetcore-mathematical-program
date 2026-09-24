// ------------------------------------------------------------------------------------------
//  <copyright file = "StartValue.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Represents the start value of a single variable within a <see cref="WarmStart{TVariable,TVariableInterval}"/>.
/// </summary>
/// <param name="Variable">The variable.</param>
/// <param name="Value">The value the variable should take in the start solution.</param>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
/// <typeparam name="TVariableInterval">The type of the variable interval's scalar, which is also the type of the start value.</typeparam>
public sealed record StartValue<TVariable, TVariableInterval>(TVariable Variable, TVariableInterval Value)
    : IStartValue<TVariable, TVariableInterval>
    where TVariable : IVariable<TVariableInterval>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Variable.Name}={Value}";
}
