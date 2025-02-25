// ------------------------------------------------------------------------------------------
//  <copyright file = "ISolverResult.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Represents the result of a solver operation on a mathematical optimization model.
/// </summary>
/// <typeparam name="TVariable">The type of the variable used in the mathematical model, constrained to implement <see cref="IVariable{TInterval}"/>.</typeparam>
/// <typeparam name="TCoefficient">The type of the coefficient used in the solver result.</typeparam>
/// <typeparam name="TVariableInterval">The type of the interval associated with the variable, constrained to implement <see cref="IScalar"/>.</typeparam>
public interface ISolverResult<in TVariable, TCoefficient, in TVariableInterval>
    where TVariable : IVariable<TVariableInterval>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
    ISolutionValues<TVariable, TCoefficient, TVariableInterval> SolutionValues { get; }
    ObjectiveValue? ObjectiveValue { get; }

    /// <summary>
    /// Indicates whether a solution to the optimization problem is feasible.
    /// </summary>
    IsFeasible IsFeasible { get; }

    /// <summary>
    /// Indicates whether the solution to the optimization problem is optimal.
    /// </summary>
    IsOptimal IsOptimal { get; }

    /// <summary>
    /// Represents the difference between the optimal solution and the best-known solution
    /// for the optimization problem. If the best bound and the objective value is zero, the gap is also zero.
    /// When the objective value is zero but best bound is not, then the gap is defined to be infinity.
    /// </summary>
    OptimalityGap? OptimalityGap { get; }

    /// <summary>
    /// Indicates whether the solver operation defaulted to using a fallback or default solver configuration.
    /// </summary>
    bool SwitchedToDefaultSolver { get; }

    /// <summary>
    /// Indicates the status of the solver's attempt to solve a mathematical optimization problem.
    /// </summary>
    SolverResultStatus SolverResultStatus { get; }
}