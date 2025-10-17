// ------------------------------------------------------------------------------------------
//  <copyright file = "OptimizationSolverExtensionTest.cs" company = "ANEXIA Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Tests.Extensions;

public sealed class OptimizationSolverExtensionTest
{
    [Fact]
    public void SolveWithoutSolverParameterUsesDefaultSolverParameter()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        var variable = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var completedModel = model.SetObjective(
            new ObjectiveFunction<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                null,
                new WeightedSum<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>().Add(variable, new RealScalar(1)),
                maximize: true));
        var solver = new FakeSolver();

        var result = solver.Solve(completedModel);

        Assert.Same(solver.Result, result);
        Assert.NotNull(solver.LastSolverParameter);
        Assert.Equal(EnableSolverOutput.False, solver.LastSolverParameter.EnableSolverOutput);
        Assert.Null(solver.LastSolverParameter.RelativeGap);
        Assert.Null(solver.LastSolverParameter.TimeLimitInMilliseconds);
        Assert.Null(solver.LastSolverParameter.NumberOfThreads);
    }

    private sealed class FakeSolver :
        IOptimizationSolver<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar, RealScalar>
    {
        internal readonly ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result =
            new SolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    ReadOnlyDictionary<IIntegerVariable<IRealScalar>, RealScalar>.Empty),
                null,
                new IsFeasible(false),
                new IsOptimal(false),
                null,
                SolverResultStatus.NotSolved,
                false);

        internal SolverParameter? LastSolverParameter { get; private set; }

        public ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Solve(
            ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> model,
            SolverParameter solverParameter)
        {
            LastSolverParameter = solverParameter;
            return Result;
        }
    }
}
