// ------------------------------------------------------------------------------------------
//  <copyright file = "ICompletedOptimizationModel.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Hint;
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

    /// <summary>
    /// Optional start solution passed to the solver as warm start / MIP start, null when none is set.
    /// Use <see cref="Extensions.CompletedOptimizationModelExtension.WithWarmStart{TVariable,TCoefficient,TVariableInterval}"/> to set one.
    /// </summary>
    public IWarmStart<TVariable, TVariableInterval>? WarmStart { get; }

    /// <summary>
    /// Optional solver specific variable attributes, null when none are set. Only applied by the native Gurobi solver.
    /// Use <see cref="Extensions.CompletedOptimizationModelExtension.WithVariableAttributes{TVariable,TCoefficient,TVariableInterval}"/> to set them.
    /// </summary>
    public IVariableAttributes<TVariable>? VariableAttributes { get; }
}