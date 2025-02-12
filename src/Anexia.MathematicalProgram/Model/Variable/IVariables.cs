// ------------------------------------------------------------------------------------------
//  <copyright file = "IVariables.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents a collection of variables.
/// </summary>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
/// <typeparam name="TInterval">The type of the variable's interval.</typeparam>
public interface IVariables<out TVariable, out TInterval> : IReadOnlyCollection<TVariable>
    where TVariable : IVariable<TInterval> where TInterval : IAddableScalar<TInterval, TInterval>
{
}