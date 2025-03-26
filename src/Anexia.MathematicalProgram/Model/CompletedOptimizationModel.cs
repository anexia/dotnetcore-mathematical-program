// ------------------------------------------------------------------------------------------
//  <copyright file = "CompletedOptimizationModel.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
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
/// <typeparam name="TInterval">
/// The scalar type associated with the variable and constraint intervals. This must implement IScalar.
/// </typeparam>
public sealed record CompletedOptimizationModel<TVariable, TCoefficient, TInterval> :
    ICompletedOptimizationModel<TVariable, TCoefficient, TInterval>
    where TVariable : IVariable<TInterval>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TInterval : IAddableScalar<TInterval, TInterval>
{
    internal CompletedOptimizationModel(IVariables<TVariable, TInterval> variables,
        IReadOnlyCollection<IConstraint<TVariable, TCoefficient, TInterval>> constraints,
        IObjectiveFunction<TVariable, TCoefficient, TInterval> objectiveFunction)
    {
        Variables = variables;
        Constraints = constraints;
        ObjectiveFunction = objectiveFunction;
    }

    /// <summary>
    /// The variables.
    /// </summary>
    public IVariables<TVariable, TInterval> Variables { get; }

    /// <summary>
    /// The constraints.
    /// </summary>
    public IReadOnlyCollection<IConstraint<TVariable, TCoefficient, TInterval>> Constraints { get; }

    /// <summary>
    /// The objective function.
    /// </summary>
    public IObjectiveFunction<TVariable, TCoefficient, TInterval> ObjectiveFunction { get; }

    public bool Equals(CompletedOptimizationModel<TVariable, TCoefficient, TInterval>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Variables.SequenceEqual(other.Variables) &&
               ObjectiveFunction.Equals(other.ObjectiveFunction) && Constraints.SequenceEqual(other.Constraints);
    }

    public override int GetHashCode() =>
        Variables.Select(item => item.GetHashCode())
            .Concat(Constraints.Select(item => item.GetHashCode()))
            .Aggregate(ObjectiveFunction.GetHashCode(),
                HashCode.Combine);

    [ExcludeFromCodeCoverage]
    public override string ToString() =>
        $"Variables: {Environment.NewLine} {string.Join(Environment.NewLine + " ", Variables)}{Environment.NewLine}Constraints: {Environment.NewLine} {string.Join(Environment.NewLine + " ", Constraints)}{Environment.NewLine}Objective function{Environment.NewLine} {ObjectiveFunction}";
}