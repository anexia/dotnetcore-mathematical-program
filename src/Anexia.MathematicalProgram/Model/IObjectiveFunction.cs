// ------------------------------------------------------------------------------------------
//  <copyright file = "IObjectiveFunction.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents an objective function (min/max offset + Sum(terms)).
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TScalar">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the variable interval's scalar.</typeparam>
public interface IObjectiveFunction<out TVariable, out TScalar, out TInterval>
    where TVariable : IVariable<TInterval>
    where TInterval : IAddableScalar<TInterval, TInterval>
    where TScalar :  IAddableScalar<TScalar, TScalar>
{
    /// <summary>
    /// The offset.
    /// </summary>
    public TScalar? Offset { get; }

    /// <summary>
    /// The terms.
    /// </summary>
    public IWeightedSum<TVariable, TScalar, TInterval> WeightedSum { get; }

    /// <summary>
    /// Boolean whether to maximize or minimize.
    /// </summary>
    public bool Maximize { get; }
}