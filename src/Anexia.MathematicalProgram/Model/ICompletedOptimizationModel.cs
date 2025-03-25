// ------------------------------------------------------------------------------------------
//  <copyright file = "ICompletedOptimizationModel.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents a completed optimization model, including variables, constraints,
/// and the objective function for mathematical optimization problems.
/// </summary>
/// <typeparam name="TVariable">
/// The type of the variables used in the optimization model. This must implement IVariable with a scalar type of TInterval.
/// </typeparam>
/// <typeparam name="TCoefficient">
/// The type of the coefficients associated with the constraints and objective function.
/// This must implement IScalar and IAddableScalar for adding scalar values.
/// </typeparam>
/// <typeparam name="TVariableInterval">
/// The scalar type associated with the variable and constraint intervals. This must implement IScalar.
/// </typeparam>
public interface ICompletedOptimizationModel<out TVariable, out TCoefficient, out TVariableInterval>
    where TVariable : IVariable<TVariableInterval>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
    public IVariables<TVariable, TVariableInterval> Variables { get; }
    public IReadOnlyCollection<IConstraint<TVariable, TCoefficient, TVariableInterval>> Constraints { get; }
    public IObjectiveFunction<TVariable, TCoefficient, TVariableInterval> ObjectiveFunction { get; }
}