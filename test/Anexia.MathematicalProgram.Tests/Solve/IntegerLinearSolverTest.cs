// ------------------------------------------------------------------------------------------
//  <copyright file = "IntegerLinearSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
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
using static Anexia.MathematicalProgram.Tests.Factory.ConstraintFactory;
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.TermFactory;
using Interval = Anexia.MathematicalProgram.Model.Interval;
using Point = Anexia.MathematicalProgram.Model.Point;

#endregion

namespace Anexia.MathematicalProgram.Tests.Solve;

public sealed class IntegerLinearSolverTest(ITestOutputHelper testOutputHelper)
{
    private Serilog.Core.Logger Logger { get; } = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo
        .TestOutput(testOutputHelper,
            outputTemplate: $"{{Timestamp:yyyy-MM-dd HH:mm:ss.fff}} [{{Level:u4}}] {{Message}}{{NewLine}}{{Exception}}",
            formatProvider: CultureInfo.InvariantCulture).CreateLogger();


    [Fact]
    public void SolverWithoutObjectiveAndConstraintsReturnsCorrectResult()
    {
        var result = new IntegerLinearProgramSolver().Solve();
        Assert.False(result.IlpIsNotFeasible());
        Assert.True(result.IsOptimal.Value);
        Assert.Equal(0, result.ObjectiveValue.Value);
        Assert.Equal(0, result.OptimalityGap.Value);
    }

    [Fact]
    public void SolverWithSimpleFeasibleILPModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=1, x binary
         */
        var result = new IntegerLinearProgramSolver()
            .SetSolverConfigurations(new SolverParameter(TimeLimitInMilliseconds.Unbounded))
            .AddIntegerVariable(Interval.BinaryInterval, "TestVariable", out var testVariable)
            .AddObjective(new Terms(Term(2, testVariable)), true)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Point.One))).Solve();

        Assert.False(result.IlpIsNotFeasible());
        Assert.True(result.IsOptimal.Value);
        Assert.Equal(2, result.ObjectiveValue.Value);
        Assert.Equal(0, result.OptimalityGap.Value);

        Logger.Information(result.SolvedSolver.ExportModelAsLpFormat(false));
    }

    [Fact]
    public void SolverWithInfeasibleILPModelReturnsCorrectResult()
    {
        /*
         * max 2x, s.t. x=3, x binary
         */
        var result = new IntegerLinearProgramSolver()
            .SetSolverConfigurations(new SolverParameter(new TimeLimitInMilliseconds(10),
                EnableSolverOutput.True,
                new NumberOfThreads(2),
                RelativeGap.EMinus7)).AddIntegerVariable(Interval.BinaryInterval, "TestVariable", out var testVariable)
            .AddObjective(new Terms(Term(2, testVariable)), false)
            .AddConstraints(Constraints(Constraint([Term(1, testVariable)], Point(3)))).Solve();

        Assert.True(result.IlpIsNotFeasible());
        Assert.False(result.IsOptimal.Value);
        Assert.Equal(double.NaN, result.ObjectiveValue.Value);
        Assert.Equal(double.NaN, result.OptimalityGap.Value);

        Logger.Information(result.SolvedSolver.ExportModelAsLpFormat(false));
    }

    [Fact]
    public void SolverWithUnboundedILPModelThrowsExpectedException()
    {
        /*
         * max 2x, x positive
         */
        var solver = new IntegerLinearProgramSolver()
            .SetSolverConfigurations(new SolverParameter(new TimeLimitInMilliseconds(10)))
            .AddIntegerVariable(Interval(0, double.PositiveInfinity), "TestVariable", out var testVariable)
            .AddObjective(new Terms(Term(2, testVariable)), false);

        var mathematicalProgramException = Assert.Throws<MathematicalProgramException>(solver.Solve);
        Logger.Error(mathematicalProgramException.Message);
    }
}