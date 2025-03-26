// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Solve;

public static class SolverFactory
{
    /// <summary>
    /// Creates an instance of a solver for solving the desired problem type.
    /// </summary>
    /// <param name="solverType">The expected underlying solver.</param>
    /// <returns>Optimization solver corresponding to the desired type</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown type is passed.</exception>
    public static
        IOptimizationSolver<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar, RealScalar>
        SolverFor(IlpSolverType solverType) =>
        solverType switch
        {
            IlpSolverType.CbcIntegerProgramming => new IlpCbcSolver(),
            IlpSolverType.GurobiIntegerProgramming or
                IlpSolverType.Scip or IlpSolverType.HiGhs => new IlpSolver(solverType),
            _ => throw new ArgumentOutOfRangeException(nameof(solverType), solverType, null)
        };

    /// <summary>
    /// Creates an instance of a solver for solving LP problems.
    /// </summary>
    /// <param name="solverType">The desired underlying solver type,</param>
    /// <returns>Optimization solver for solving desired problems.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown type is passed.</exception>
    public static
        IOptimizationSolver<ContinuousVariable<IRealScalar>, IRealScalar, IRealScalar, RealScalar>
        SolverFor(LpSolverType solverType) =>
        solverType switch
        {
            LpSolverType.Glop => new LpSolver(solverType),
            LpSolverType.Scip => new LpSolver(solverType),
            LpSolverType.GurobiMixedIntegerProgramming => new LpSolver(solverType),
            _ => throw new ArgumentOutOfRangeException(nameof(solverType), solverType, null)
        };

    /// <summary>
    /// Creates an optimization solver for solving CP models. At the moment, CP-SAT from Google OR Tools is used.
    /// </summary>
    /// <returns>Optimization solver with underlying CP-SAT solver, see <see href="https://developers.google.com/optimization/cp/cp_solver"/></returns>
    public static IOptimizationSolver<IIntegerVariable<IIntegerScalar>, IIntegerScalar, IIntegerScalar, IntegerScalar>
        NewCpSolver() =>
        new ConstraintProgrammingSolver();
}