// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverResult.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Represents the solver's result.
/// </summary>
/// <param name="SolutionValues">The solution value.</param>
/// <param name="ObjectiveValue">The objective value.</param>
/// <param name="IsFeasible">Information, whether the model was feasible, or not.</param>
/// <param name="IsOptimal">Information, whether the solution is optimal, or not.</param>
/// <param name="OptimalityGap">Represents the difference between the optimal solution and the best-known solution
/// for the optimization problem. If the best bound and the objective value is zero, the gap is also zero.
/// When the objective value is zero but best bound is not, then the gap is defined to be infinity.</param>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TVariableInterval">The type of the variable interval's scalar.</typeparam>
public readonly record struct SolverResult<TVariable, TCoefficient, TVariableInterval>(
    ISolutionValues<TVariable, TCoefficient, TVariableInterval> SolutionValues,
    ObjectiveValue? ObjectiveValue,
    IsFeasible IsFeasible,
    IsOptimal IsOptimal,
    OptimalityGap? OptimalityGap,
    SolverResultStatus SolverResultStatus,
    bool SwitchedToDefaultSolver) : ISolverResult<TVariable, TCoefficient, TVariableInterval>
    where TVariable : IVariable<TVariableInterval>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
}