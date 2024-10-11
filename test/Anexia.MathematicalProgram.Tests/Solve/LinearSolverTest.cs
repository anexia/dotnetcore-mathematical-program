// ------------------------------------------------------------------------------------------
//  <copyright file = "LinearSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using System.Globalization;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
using Serilog;
using Xunit.Abstractions;
using static System.Double;
using static Anexia.MathematicalProgram.Tests.Factory.ConstraintFactory;
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.TermFactory;

#endregion

namespace Anexia.MathematicalProgram.Tests.Solve;

public sealed class LinearSolverTest(ITestOutputHelper testOutputHelper)
{
    private Serilog.Core.Logger Logger { get; } = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo
        .TestOutput(testOutputHelper,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u4}] {Message}{NewLine}{Exception}",
            formatProvider: CultureInfo.InvariantCulture).CreateLogger();

    [Fact]
    public void SolverWithoutObjectiveAndConstraintsReturnsCorrectResult()
    {
        var result = new LinearProgramSolver().Solve();
        Assert.False(result.IlpIsNotFeasible());
        Assert.True(result.IsOptimal.Value);
        Assert.Equal(0, result.ObjectiveValue.Value);
        Assert.Equal(0, result.OptimalityGap.Value);
    }

    [Fact]
    public void SolverWithSimpleFeasibleMinimizationLPModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=2, x in (0,3)
         */
        var result = new LinearProgramSolver()
            .SetSolverConfigurations(new SolverParameter(TimeLimitInMilliseconds.Unbounded))
            .AddContinuousVariable(Interval(0, 3), "TestVariable", out var testVariable)
            .AddObjective(new Terms(Term(2, testVariable)), true)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Point(2)))).Solve();

        Assert.False(result.IlpIsNotFeasible());
        Assert.True(result.IsOptimal.Value);
        Assert.Equal(4, result.ObjectiveValue.Value);
        Assert.Equal(0, result.OptimalityGap.Value);

        Logger.Information(result.SolvedSolver.ExportModelAsLpFormat(false));
    }

    [Fact]
    public void SolverWithSimpleFeasibleMaximizationLPModelReturnsCorrectResult()
    {
        /*
         * max 2x, s.t. x<=2, x in (0,3)
         */
        var result = new LinearProgramSolver()
            .SetSolverConfigurations(new SolverParameter(TimeLimitInMilliseconds.Unbounded))
            .AddContinuousVariable(Interval(0, 3), "TestVariable", out var testVariable)
            .AddObjective(new Terms(Term(2, testVariable)), false)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Interval(NegativeInfinity, 2)))).Solve();

        Assert.False(result.IlpIsNotFeasible());
        Assert.True(result.IsOptimal.Value);
        Assert.Equal(4, result.ObjectiveValue.Value);
        Assert.Equal(0, result.OptimalityGap.Value);

        Logger.Information(result.SolvedSolver.ExportModelAsLpFormat(false));
    }

    [Fact]
    public void SolverWithInfeasibleLPModelReturnsCorrectResult()
    {
        /*
         * max 2x, s.t. x=3, x in (0,1)
         */
        var result = new LinearProgramSolver()
            .SetSolverConfigurations(new SolverParameter(new TimeLimitInMilliseconds(10),
                EnableSolverOutput.True,
                new NumberOfThreads(2),
                RelativeGap.EMinus7)).AddContinuousVariable(Interval(0, 1), "TestVariable", out var testVariable)
            .AddObjective(new Terms(Term(2, testVariable)), false)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Point(3)))).Solve();

        Assert.True(result.IlpIsNotFeasible());
        Assert.False(result.IsOptimal.Value);
        Assert.Equal(NaN, result.ObjectiveValue.Value);
        Assert.Equal(NaN, result.OptimalityGap.Value);

        Logger.Information(result.SolvedSolver.ExportModelAsLpFormat(false));
    }

    [Fact]
    public void SolverWithAbnormalLPModelThrowsExpectedException()
    {
        /*
         * min infinity x, x in R
         */
        var solver = new LinearProgramSolver()
            .SetSolverConfigurations(new SolverParameter(new TimeLimitInMilliseconds(10)))
            .AddContinuousVariable(Interval(NegativeInfinity, PositiveInfinity), "TestVariable", out var testVariable)
            .AddObjective(new Terms(Term(NegativeInfinity, testVariable)), true);

        var mathematicalProgramException = Assert.Throws<MathematicalProgramException>(solver.Solve);
        Logger.Error(mathematicalProgramException.Message);
    }
}