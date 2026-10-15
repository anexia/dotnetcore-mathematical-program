// ------------------------------------------------------------------------------------------
//  <copyright file = "StartSolutionSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Hint;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
using Microsoft.Extensions.Logging;
using static Anexia.MathematicalProgram.Tests.Factory.SolutionValuesFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolverResultFactory;

namespace Anexia.MathematicalProgram.Tests.Solve;

/// <summary>
/// Tests passing a warm start and variable attributes through the different solvers.
/// Model: max x + 2y, s.t. x + y <= 1, x and y binary. Optimum: x = 0, y = 1 with objective value 2.
/// The suboptimal but feasible solution x = 1, y = 0 is used as warm start.
/// </summary>
public sealed class StartSolutionSolverTest
{
    private const string SolutionHintMessage = "Setting solution hint for 2 variables";
    private const string IgnoredStartSolutionMessage = "ignoring warm start for 2 variables";

    [Fact]
    public void IlpSolverWithSuboptimalStartSolutionReturnsOptimalResult()
    {
        var model = CreateIlpModel(out var x, out var y).WithHint(SuboptimalStartSolution(x, y));

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithInfeasibleStartSolutionReturnsOptimalResult()
    {
        // x = 1, y = 1 violates x + y <= 1.
        var model = CreateIlpModel(out var x, out var y).WithHint(
            new StartSolution<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, BinaryScalar.One).Add(y, BinaryScalar.One));

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithPartialStartSolutionReturnsOptimalResult()
    {
        var model = CreateIlpModel(out var x, out var y).WithHint(
            new StartSolution<IIntegerVariable<IRealScalar>, IRealScalar>().Add(y, BinaryScalar.One));

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithStartSolutionLogsNumberOfHintedVariables() =>
        Assert.Contains(SolveWithScipAndLogger(withStartSolution: true).Logger.Entries,
            entry => entry.Message.Contains(SolutionHintMessage));

    [Fact]
    public void IlpSolverWithoutStartSolutionDoesNotLogSolutionHint() =>
        Assert.DoesNotContain(SolveWithScipAndLogger(withStartSolution: false).Logger.Entries,
            entry => entry.Message.Contains("solution hint"));

    [Fact]
    public void IlpSolverWithoutStartSolutionAndAttributesDoesNotLogWarning() =>
        Assert.DoesNotContain(SolveWithScipAndLogger(withStartSolution: false).Logger.Entries,
            entry => entry.Level == LogLevel.Warning);

    [Fact]
    public void IlpSolverWithVariableAttributesReturnsOptimalResult()
    {
        var (result, _, x, y) = SolveWithScipAndLogger(withStartSolution: false, withAttributes: true);

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithVariableAttributesLogsWarningThatAttributesAreIgnored() =>
        Assert.Contains(SolveWithScipAndLogger(withStartSolution: false, withAttributes: true).Logger.Entries,
            entry => entry.Level == LogLevel.Warning && entry.Message.Contains("ignoring 2 attributes"));

    [Fact]
    public void IlpSolverWithHiGhsAndStartSolutionReturnsOptimalResult()
    {
        var (result, _, x, y) = SolveWithHiGhsAndStartSolution();

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithHiGhsLogsWarningThatStartSolutionIsIgnored() =>
        Assert.Contains(SolveWithHiGhsAndStartSolution().Logger.Entries,
            entry => entry.Level == LogLevel.Warning && entry.Message.Contains(IgnoredStartSolutionMessage));

    [Fact]
    public void IlpSolverWithHiGhsDoesNotForwardStartSolution() =>
        Assert.DoesNotContain(SolveWithHiGhsAndStartSolution().Logger.Entries,
            entry => entry.Message.Contains("Setting solution hint"));

    [Fact]
    public void IlpSolverWithHiGhsAsFallbackReturnsOptimalObjectiveValue() =>
        Assert.Equal(new ObjectiveValue(2), SolveWithHiGhsAsFallbackAndStartSolution().Result.ObjectiveValue);

    [Fact]
    public void IlpSolverWithHiGhsAsFallbackSwitchesSolver() =>
        Assert.True(SolveWithHiGhsAsFallbackAndStartSolution().Result.SwitchedToDefaultSolver);

    [Fact]
    public void IlpSolverWithHiGhsAsFallbackLogsWarningThatStartSolutionIsIgnored() =>
        Assert.Contains(SolveWithHiGhsAsFallbackAndStartSolution().Logger.Entries,
            entry => entry.Level == LogLevel.Warning && entry.Message.Contains(IgnoredStartSolutionMessage));

    [Fact]
    public void StartSolutionFromPreviousSolutionContainsAllVariables() =>
        Assert.Equal(2, StartSolutionFromPreviousSolution(out _, out _).Count);

    [Fact]
    public void StartSolutionFromPreviousSolutionEqualsOptimalSolution()
    {
        var startSolution = StartSolutionFromPreviousSolution(out var x, out var y);

        Assert.Equal(
            new StartSolution<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new RealScalar(0)).Add(y, new RealScalar(1)),
            startSolution);
    }

    [Fact]
    public void ResolvingWithPreviousSolutionAsStartSolutionReturnsOptimalResult()
    {
        var startSolution = StartSolutionFromPreviousSolution(out var x, out var y);
        var model = CreateIlpModel(out _, out _).WithHint(startSolution);

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void CpSolverWithSuboptimalStartSolutionReturnsOptimalResult()
    {
        var model = new OptimizationModel<IIntegerVariable<IIntegerScalar>, IIntegerScalar, IIntegerScalar>();
        var x = model.NewBinaryVariable<BinaryVariable>("x");
        var y = model.NewBinaryVariable<BinaryVariable>("y");
        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).AddTermToSum(new IntegerScalar(1), y)
            .Build(new IntegralInterval(0, 1)));
        var completedModel = model.SetObjective(model.CreateObjectiveFunctionBuilder()
                .AddTermToSum(new IntegerScalar(1), x).AddTermToSum(new IntegerScalar(2), y).Build())
            .WithHint(new StartSolution<IIntegerVariable<IIntegerScalar>, IIntegerScalar>()
                .Add(x, BinaryScalar.One).Add(y, BinaryScalar.Zero));

        var result = SolverFactory.NewCpSolver().Solve(completedModel, SilentParameter());

        Assert.Equal(
            SolverResult(
                SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(
                    (x, new IntegerScalar(0)), (y, new IntegerScalar(1))),
                new ObjectiveValue(2), new IsFeasible(true), new IsOptimal(true), new OptimalityGap(0),
                SolverResultStatus.Optimal, false), result);
    }

    [Theory]
    [InlineData(LpSolverType.Glop)]
    [InlineData(LpSolverType.Scip)]
    public void LpSolverWithStartSolutionReturnsOptimalResult(LpSolverType solverType)
    {
        /*
         * max x + 2y, s.t. x + y <= 1, x and y in [0, 1]. Optimum: x = 0, y = 1.
         */
        var model = new OptimizationModel<ContinuousVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<ContinuousVariable<IRealScalar>>(new RealInterval(0, 1), "x");
        var y = model.NewVariable<ContinuousVariable<IRealScalar>>(new RealInterval(0, 1), "y");
        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).AddTermToSum(new IntegerScalar(1), y)
            .Build(new RealInterval(0, 1)));
        var completedModel = model.SetObjective(model.CreateObjectiveFunctionBuilder()
                .AddTermToSum(new IntegerScalar(1), x).AddTermToSum(new IntegerScalar(2), y).Build())
            .WithHint(new StartSolution<ContinuousVariable<IRealScalar>, IRealScalar>()
                .Add(x, new RealScalar(1)).Add(y, new RealScalar(0)));

        var result = SolverFactory.SolverFor(solverType).Solve(completedModel, SilentParameter());

        Assert.Equal(
            SolverResult(
                SolutionValues<ContinuousVariable<IRealScalar>, RealScalar, IRealScalar>(
                    (x, new RealScalar(0)), (y, new RealScalar(1))),
                new ObjectiveValue(2), new IsFeasible(true), new IsOptimal(true), null,
                SolverResultStatus.Optimal, false), result);
    }

    // With a solution limit of one and presolve disabled, Gurobi stops right after the MIP start was accepted,
    // so the returned solution must be the (suboptimal) warm start.

    [RequiresGurobiLicenceFact]
    public void GurobiNativeSolverWithSolutionLimitReturnsObjectiveValueOfStartSolution() =>
        Assert.Equal(new ObjectiveValue(1), SolveWithGurobiAndSolutionLimit().Result.ObjectiveValue);

    [RequiresGurobiLicenceFact]
    public void GurobiNativeSolverWithSolutionLimitReturnsFeasibleStatus() =>
        Assert.Equal(SolverResultStatus.Feasible, SolveWithGurobiAndSolutionLimit().Result.SolverResultStatus);

    [RequiresGurobiLicenceFact]
    public void GurobiNativeSolverWithSolutionLimitReturnsStartSolutionValues()
    {
        var (result, x, y) = SolveWithGurobiAndSolutionLimit();

        Assert.Equal(
            SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                (x, new RealScalar(1)), (y, new RealScalar(0))),
            result.SolutionValues);
    }

    [RequiresGurobiLicenceFact]
    public void GurobiNativeSolverWithStartSolutionAndAttributesReturnsOptimalResult()
    {
        var (result, _, x, y) = SolveWithGurobiAndAttributes();

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [RequiresGurobiLicenceFact]
    public void GurobiNativeSolverLogsNumberOfMipStartVariables() =>
        Assert.Contains(SolveWithGurobiAndAttributes().Logger.Entries,
            entry => entry.Message.Contains("Setting MIP start for 2 variables"));

    [RequiresGurobiLicenceFact]
    public void GurobiNativeSolverLogsNumberOfVariableAttributes() =>
        Assert.Contains(SolveWithGurobiAndAttributes().Logger.Entries,
            entry => entry.Message.Contains("Setting 4 variable attributes"));

    [RequiresNoGurobiLicenceFact]
    public void GurobiNativeSolverWithoutLicenceReturnsOptimalResultFromFallbackSolver()
    {
        var (result, _, x, y) = SolveWithGurobiFallingBackToScip();

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [RequiresNoGurobiLicenceFact]
    public void GurobiNativeSolverWithoutLicenceForwardsStartSolutionToFallbackSolver() =>
        Assert.Contains(SolveWithGurobiFallingBackToScip().Logger.Entries,
            entry => entry.Message.Contains(SolutionHintMessage));

    [RequiresNoGurobiLicenceFact]
    public void GurobiNativeSolverWithoutLicenceLogsThatAttributesAreIgnoredByFallbackSolver() =>
        Assert.Contains(SolveWithGurobiFallingBackToScip().Logger.Entries,
            entry => entry.Message.Contains("ignoring 1 attributes"));

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result, FakeLogger Logger,
        IIntegerVariable<IRealScalar> X, IIntegerVariable<IRealScalar> Y) SolveWithScipAndLogger(
        bool withStartSolution, bool withAttributes = false)
    {
        var logger = new FakeLogger();
        var model = CreateIlpModel(out var x, out var y);
        if (withStartSolution) model = model.WithHint(SuboptimalStartSolution(x, y));
        if (withAttributes)
        {
            model = model.WithVariableAttributes(new VariableAttributes<IIntegerVariable<IRealScalar>>()
                .Add(x, VariableAttributeType.HintValue, 1)
                .Add(y, VariableAttributeType.BranchPriority, 5));
        }

        var result = new IlpSolver(IlpSolverType.Scip, logger: logger).Solve(model, SilentParameter());

        return (result, logger, x, y);
    }

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result, FakeLogger Logger,
        IIntegerVariable<IRealScalar> X, IIntegerVariable<IRealScalar> Y) SolveWithHiGhsAndStartSolution()
    {
        var logger = new FakeLogger();
        var model = CreateIlpModel(out var x, out var y).WithHint(SuboptimalStartSolution(x, y));

        var result = new IlpSolver(IlpSolverType.HiGhs, logger: logger).Solve(model, SilentParameter());

        return (result, logger, x, y);
    }

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result, FakeLogger Logger)
        SolveWithHiGhsAsFallbackAndStartSolution()
    {
        var logger = new FakeLogger();
        var model = CreateIlpModel(out var x, out var y).WithHint(SuboptimalStartSolution(x, y));

        var result = new IlpSolver((IlpSolverType)int.MaxValue, IlpSolverType.HiGhs, logger)
            .Solve(model, SilentParameter());

        return (result, logger);
    }

    private static StartSolution<IIntegerVariable<IRealScalar>, IRealScalar> StartSolutionFromPreviousSolution(
        out IIntegerVariable<IRealScalar> x, out IIntegerVariable<IRealScalar> y)
    {
        var model = CreateIlpModel(out x, out y);

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        return ((SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>)result.SolutionValues)
            .ToStartSolution();
    }

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result,
        IIntegerVariable<IRealScalar> X, IIntegerVariable<IRealScalar> Y) SolveWithGurobiAndSolutionLimit()
    {
        var model = CreateIlpModel(out var x, out var y).WithHint(SuboptimalStartSolution(x, y));

        var result = new GurobiNativeSolver().Solve(model, new SolverParameter(EnableSolverOutput.False,
            AdditionalSolverSpecificParameters: [("SolutionLimit", "1"), ("Presolve", "0")]));

        return (result, x, y);
    }

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result, FakeLogger Logger,
        IIntegerVariable<IRealScalar> X, IIntegerVariable<IRealScalar> Y) SolveWithGurobiAndAttributes()
    {
        var logger = new FakeLogger();
        var model = CreateIlpModel(out var x, out var y)
            .WithHint(SuboptimalStartSolution(x, y))
            .WithVariableAttributes(new VariableAttributes<IIntegerVariable<IRealScalar>>()
                .Add(x, VariableAttributeType.HintValue, 0)
                .Add(x, VariableAttributeType.HintPriority, 2)
                .Add(y, VariableAttributeType.HintValue, 1)
                .Add(y, VariableAttributeType.BranchPriority, 7));

        var result = new GurobiNativeSolver(logger).Solve(model, SilentParameter());

        return (result, logger, x, y);
    }

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result, FakeLogger Logger,
        IIntegerVariable<IRealScalar> X, IIntegerVariable<IRealScalar> Y) SolveWithGurobiFallingBackToScip()
    {
        var logger = new FakeLogger();
        var model = CreateIlpModel(out var x, out var y)
            .WithHint(SuboptimalStartSolution(x, y))
            .WithVariableAttributes(new VariableAttributes<IIntegerVariable<IRealScalar>>()
                .Add(x, VariableAttributeType.HintValue, 0));

        var result = new IlpSolver(IlpSolverType.GurobiNativeIntegerProgramming, IlpSolverType.Scip, logger)
            .Solve(model, SilentParameter());

        return (result, logger, x, y);
    }

    private static SolverParameter SilentParameter() => new(EnableSolverOutput.False);

    private static ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar> CreateIlpModel(
        out IIntegerVariable<IRealScalar> x, out IIntegerVariable<IRealScalar> y)
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        x = model.NewBinaryVariable<BinaryVariable>("x");
        y = model.NewBinaryVariable<BinaryVariable>("y");

        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).AddTermToSum(new IntegerScalar(1), y)
            .Build(new RealInterval(0, 1)));

        return model.SetObjective(model.CreateObjectiveFunctionBuilder()
            .AddTermToSum(new IntegerScalar(1), x).AddTermToSum(new IntegerScalar(2), y).Build());
    }

    private static StartSolution<IIntegerVariable<IRealScalar>, IRealScalar> SuboptimalStartSolution(
        IIntegerVariable<IRealScalar> x, IIntegerVariable<IRealScalar> y) =>
        new StartSolution<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, BinaryScalar.One).Add(y, BinaryScalar.Zero);

    private static SolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> OptimalIlpResult(
        IIntegerVariable<IRealScalar> x, IIntegerVariable<IRealScalar> y) =>
        SolverResult(
            SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                (x, new RealScalar(0)), (y, new RealScalar(1))),
            new ObjectiveValue(2), new IsFeasible(true), new IsOptimal(true), new OptimalityGap(0),
            SolverResultStatus.Optimal, false);

    private sealed class FakeLogger : ILogger<IlpSolver>
    {
        internal List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            Entries.Add((logLevel, formatter(state, exception)));
    }
}
