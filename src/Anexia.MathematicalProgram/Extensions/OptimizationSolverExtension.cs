// ------------------------------------------------------------------------------------------
//  <copyright file = "OptimizationSolverExtension.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Extensions;

public static class OptimizationSolverExtension
{
    /// <summary>
    /// Solves the given model with default solver parameter.
    /// </summary>
    /// <param name="solver">The solver.</param>
    /// <param name="model">The model to be solved.</param>
    /// <returns>Solver result containing solution information.</returns>
    public static ISolverResult<TVariable, TSolutionType, TIntervalScalar> Solve<TVariable, TCoefficient,
        TIntervalScalar, TSolutionType>(
        this IOptimizationSolver<TVariable, TCoefficient, TIntervalScalar, TSolutionType> solver,
        ICompletedOptimizationModel<TVariable, TCoefficient, TIntervalScalar> model)
        where TIntervalScalar : IAddableScalar<TIntervalScalar, TIntervalScalar>
        where TVariable : IVariable<TIntervalScalar>
        where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
        where TSolutionType : IAddableScalar<TSolutionType, TSolutionType> =>
        solver.Solve(model, new SolverParameter());
}