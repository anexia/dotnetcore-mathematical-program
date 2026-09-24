// ------------------------------------------------------------------------------------------
//  <copyright file = "IStartValue.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Represents the start value of a single variable within a <see cref="IWarmStart{TVariable,TVariableInterval}"/>.
/// </summary>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
/// <typeparam name="TVariableInterval">The type of the variable interval's scalar, which is also the type of the start value.</typeparam>
public interface IStartValue<out TVariable, out TVariableInterval>
    where TVariable : IVariable<TVariableInterval>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
    /// <summary>
    /// The variable.
    /// </summary>
    public TVariable Variable { get; }

    /// <summary>
    /// The value the variable should take in the start solution.
    /// </summary>
    public TVariableInterval Value { get; }
}
