// ------------------------------------------------------------------------------------------
//  <copyright file = "CompletedOptimizationModelExtension.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Hint;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;

namespace Anexia.MathematicalProgram.Extensions;

public static class CompletedOptimizationModelExtension
{
    /// <summary>
    /// Returns a copy of the model that passes the given start solution to the solver as warm start / MIP start.
    /// The start solution may be partial. Replaces a previously set warm start.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="warmStart">The start solution.</param>
    /// <returns>A new model containing the warm start.</returns>
    /// <exception cref="VariableNotInModelException{T}">Thrown when the warm start contains a variable that is not
    /// part of the model.</exception>
    public static ICompletedOptimizationModel<TVariable, TCoefficient, TIntervalScalar> WithWarmStart<TVariable,
        TCoefficient, TIntervalScalar>(
        this ICompletedOptimizationModel<TVariable, TCoefficient, TIntervalScalar> model,
        IWarmStart<TVariable, TIntervalScalar> warmStart)
        where TVariable : IVariable<TIntervalScalar>
        where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
        where TIntervalScalar : IAddableScalar<TIntervalScalar, TIntervalScalar>
    {
        EnsureVariablesArePartOfModel(model, warmStart.Select(startValue => startValue.Variable));

        return new CompletedOptimizationModel<TVariable, TCoefficient, TIntervalScalar>(model.Variables,
            model.Constraints, model.ObjectiveFunction, warmStart, model.VariableAttributes);
    }

    /// <summary>
    /// Returns a copy of the model that passes the given solver specific variable attributes to the solver.
    /// Only the native Gurobi solver applies them, all other solvers ignore them. Replaces previously set attributes.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="variableAttributes">The variable attributes.</param>
    /// <returns>A new model containing the variable attributes.</returns>
    /// <exception cref="VariableNotInModelException{T}">Thrown when the attributes contain a variable that is not
    /// part of the model.</exception>
    public static ICompletedOptimizationModel<TVariable, TCoefficient, TIntervalScalar> WithVariableAttributes<
        TVariable, TCoefficient, TIntervalScalar>(
        this ICompletedOptimizationModel<TVariable, TCoefficient, TIntervalScalar> model,
        IVariableAttributes<TVariable> variableAttributes)
        where TVariable : IVariable<TIntervalScalar>
        where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
        where TIntervalScalar : IAddableScalar<TIntervalScalar, TIntervalScalar>
    {
        EnsureVariablesArePartOfModel(model, variableAttributes.Select(attribute => attribute.Variable));

        return new CompletedOptimizationModel<TVariable, TCoefficient, TIntervalScalar>(model.Variables,
            model.Constraints, model.ObjectiveFunction, model.WarmStart, variableAttributes);
    }

    /// <summary>
    /// Creates a warm start from the given solution values, e.g., to pass the solution of a previous run as start
    /// solution to the solver.
    /// </summary>
    /// <param name="solutionValues">The solution values of a previous solver run.</param>
    /// <returns>Warm start containing all variables of the solution values.</returns>
    public static WarmStart<TVariable, TIntervalScalar> ToWarmStart<TVariable, TScalar, TIntervalScalar>(
        this SolutionValues<TVariable, TScalar, TIntervalScalar> solutionValues)
        where TVariable : IVariable<TIntervalScalar>
        where TScalar : TIntervalScalar
        where TIntervalScalar : IAddableScalar<TIntervalScalar, TIntervalScalar> =>
        new(solutionValues.Select(pair =>
            new KeyValuePair<TVariable, TIntervalScalar>(pair.Key, pair.Value)));

    private static void EnsureVariablesArePartOfModel<TVariable, TCoefficient, TIntervalScalar>(
        ICompletedOptimizationModel<TVariable, TCoefficient, TIntervalScalar> model,
        IEnumerable<TVariable> variables)
        where TVariable : IVariable<TIntervalScalar>
        where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
        where TIntervalScalar : IAddableScalar<TIntervalScalar, TIntervalScalar>
    {
        var modelVariables = model.Variables.ToHashSet();

        var unknownVariable = variables.FirstOrDefault(variable => !modelVariables.Contains(variable));
        if (unknownVariable is not null)
            throw new VariableNotInModelException<TIntervalScalar>(unknownVariable);
    }
}
