// ------------------------------------------------------------------------------------------
//  <copyright file = "IntegerLinearSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
using static Anexia.MathematicalProgram.Tests.Factory.ConstraintFactory;
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.TermFactory;
using Interval = Anexia.MathematicalProgram.Model.Interval;
using Point = Anexia.MathematicalProgram.Model.Point;

#endregion

namespace Anexia.MathematicalProgram.Tests.Solve;

public sealed class IntegerLinearSolverTest
{
    [Fact]
    public void SolverReturnsCorrectIlpSolverType()
    {
        var solver = new IntegerLinearProgramSolver();

        Assert.Equal(IlpSolverType.CbcMixedIntegerProgramming, solver.SolverType);
    }
    
    [Fact]
    public void SolverWithoutObjectiveAndConstraintsReturnsCorrectResult()
    {
        var result = new IntegerLinearProgramSolver().Solve();

        Assert.Equal(
            new SolverResult(result.SolvedSolver, new ObjectiveValue(0), new IsFeasible(true), new IsOptimal(true),
                new OptimalityGap(0)), result);
    }

    [Fact]
    public void SolverWithSimpleFeasibleIlpModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=1, x binary
         */
        var result = new IntegerLinearProgramSolver()
            .AddIntegerVariable(Interval.BinaryInterval, "TestVariable", out var testVariable)
            .SetObjective(new Terms(Term(2, testVariable)), true)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Point.One))).Solve();

        Assert.Equal(new SolverResult(
            result.SolvedSolver, new ObjectiveValue(2), new IsFeasible(true), new IsOptimal(true),
            new OptimalityGap(0)), result);
    }

    [Fact]
    public void SolverWithInfeasibleIlModelReturnsCorrectResult()
    {
        /*
         * max 2x, s.t. x=3, x binary
         */
        var result = new IntegerLinearProgramSolver()
            .AddIntegerVariable(Interval.BinaryInterval, "TestVariable", out var testVariable)
            .SetObjective(new Terms(Term(2, testVariable)), false)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Point(3)))).Solve(new SolverParameter(
                EnableSolverOutput.True,
                RelativeGap.EMinus7,
                new TimeLimitInMilliseconds(10),
                new NumberOfThreads(2)));

        Assert.Equal(new SolverResult(
            result.SolvedSolver, new ObjectiveValue(double.NaN), new IsFeasible(false), new IsOptimal(false),
            new OptimalityGap(double.NaN)), result);
    }

    [Fact]
    public void SolverWithUnboundedIlModelThrowsExpectedException()
    {
        /*
         * max 2x, x positive
         */
        var solver = new IntegerLinearProgramSolver()
            .AddIntegerVariable(Interval(0, double.PositiveInfinity), "TestVariable", out var testVariable)
            .SetObjective(new Terms(Term(2, testVariable)), false);

        Assert.Throws<MathematicalProgramException>(() =>
            solver.Solve(new SolverParameter(new TimeLimitInMilliseconds(10))));
    }
}