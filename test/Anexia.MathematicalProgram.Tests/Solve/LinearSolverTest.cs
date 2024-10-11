// ------------------------------------------------------------------------------------------
//  <copyright file = "LinearSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
using static System.Double;
using static Anexia.MathematicalProgram.Tests.Factory.ConstraintFactory;
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.TermFactory;

#endregion

namespace Anexia.MathematicalProgram.Tests.Solve;

public sealed class LinearSolverTest
{
    [Fact]
    public void SolverWithoutObjectiveAndConstraintsReturnsCorrectResult()
    {
        var result = new LinearProgramSolver().Solve();

        Assert.Equal(new SolverResult(
            result.SolvedSolver, new ObjectiveValue(0), new IsFeasible(true), new IsOptimal(true),
            new OptimalityGap(0)), result);
    }

    [Fact]
    public void SolverWithSimpleFeasibleMinimizationLpModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=2, x in (0,3)
         */
        var result = new LinearProgramSolver()
            .AddContinuousVariable(Interval(0, 3), "TestVariable", out var testVariable)
            .SetObjective(new Terms(Term(2, testVariable)), true)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Point(2)))).Solve();

        Assert.Equal(new SolverResult(
            result.SolvedSolver, new ObjectiveValue(4), new IsFeasible(true), new IsOptimal(true),
            new OptimalityGap(0)), result);
    }

    [Fact]
    public void SolverWithSimpleFeasibleMaximizationLpModelReturnsCorrectResult()
    {
        /*
         * max 2x, s.t. x<=2, x in (0,3)
         */
        var result = new LinearProgramSolver()
            .AddContinuousVariable(Interval(0, 3), "TestVariable", out var testVariable)
            .SetObjective(new Terms(Term(2, testVariable)), false)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Interval(NegativeInfinity, 2)))).Solve();

        Assert.Equal(new SolverResult(
            result.SolvedSolver, new ObjectiveValue(4), new IsFeasible(true), new IsOptimal(true),
            new OptimalityGap(0)), result);
    }

    [Fact]
    public void SolverWithInfeasibleLpModelReturnsCorrectResult()
    {
        /*
         * max 2x, s.t. x=3, x in (0,1)
         */
        var result = new LinearProgramSolver()
            .AddContinuousVariable(Interval(0, 1), "TestVariable", out var testVariable)
            .SetObjective(new Terms(Term(2, testVariable)), false)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Point(3)))).Solve(new SolverParameter(
                EnableSolverOutput.True,
                RelativeGap.EMinus7, new TimeLimitInMilliseconds(10),
                new NumberOfThreads(2)));

        Assert.Equal(new SolverResult(
            result.SolvedSolver, new ObjectiveValue(double.NaN), new IsFeasible(false), new IsOptimal(false),
            new OptimalityGap(double.NaN)), result);
    }

    [Fact]
    public void SolverWithAbnormalLpModelThrowsExpectedException()
    {
        /*
         * min infinity x, x in R
         */
        var solver = new LinearProgramSolver()
            .AddContinuousVariable(Interval(NegativeInfinity, PositiveInfinity), "TestVariable", out var testVariable)
            .SetObjective(new Terms(Term(NegativeInfinity, testVariable)), true);

        Assert.Throws<MathematicalProgramException>(() =>
            solver.Solve(new SolverParameter(new TimeLimitInMilliseconds(10))));
    }
}