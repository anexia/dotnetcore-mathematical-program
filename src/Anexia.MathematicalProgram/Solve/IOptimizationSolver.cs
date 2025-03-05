// ------------------------------------------------------------------------------------------
//  <copyright file = "IOptimizationSolver.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// Represents a solver for solving optimization problems.
/// For LP and ILP, SCIP is used as default solver when the desired one cannot be used.
/// </summary>
/// <typeparam name="TVariable">The type of the model's variables.</typeparam>
/// <typeparam name="TCoefficient">The type of the variable's coefficients.</typeparam>
/// <typeparam name="TIntervalScalar">The type of the variable's and constraint's interval scalar.</typeparam>
/// <typeparam name="TSolutionType">The type of the solution variable's values.</typeparam>
public interface IOptimizationSolver<in TVariable, in TCoefficient, in TIntervalScalar, TSolutionType>
    where TVariable : IVariable<TIntervalScalar>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TIntervalScalar : IAddableScalar<TIntervalScalar, TIntervalScalar>
    where TSolutionType : IAddableScalar<TSolutionType, TSolutionType>
{
    /// <summary>
    /// Solves the given model.
    /// </summary>
    /// <param name="model">The model to be solved.</param>
    /// <param name="solverParameter">Parameters to be passed to the underlying solver.</param>
    /// <returns>Solver result containing solution information.</returns>
    public ISolverResult<TVariable, TSolutionType, TIntervalScalar> Solve(
        ICompletedOptimizationModel<TVariable, TCoefficient, TIntervalScalar> model,
        SolverParameter solverParameter);

}