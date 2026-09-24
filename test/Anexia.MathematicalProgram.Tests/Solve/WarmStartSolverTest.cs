// ------------------------------------------------------------------------------------------
//  <copyright file = "WarmStartSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
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
public sealed class WarmStartSolverTest
{
    private const string SolutionHintMessage = "Setting solution hint for 2 variables";
    private const string IgnoredWarmStartMessage = "ignoring warm start for 2 variables";

    [Fact]
    public void IlpSolverWithSuboptimalWarmStartReturnsOptimalResult()
    {
        var model = CreateIlpModel(out var x, out var y).WithWarmStart(SuboptimalWarmStart(x, y));

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithInfeasibleWarmStartReturnsOptimalResult()
    {
        // x = 1, y = 1 violates x + y <= 1.
        var model = CreateIlpModel(out var x, out var y).WithWarmStart(
            new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, BinaryScalar.One).Add(y, BinaryScalar.One));

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithPartialWarmStartReturnsOptimalResult()
    {
        var model = CreateIlpModel(out var x, out var y).WithWarmStart(
            new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(y, BinaryScalar.One));

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithWarmStartLogsNumberOfHintedVariables() =>
        Assert.Contains(SolveWithScipAndLogger(withWarmStart: true).Logger.Entries,
            entry => entry.Message.Contains(SolutionHintMessage));

    [Fact]
    public void IlpSolverWithoutWarmStartDoesNotLogSolutionHint() =>
        Assert.DoesNotContain(SolveWithScipAndLogger(withWarmStart: false).Logger.Entries,
            entry => entry.Message.Contains("solution hint"));

    [Fact]
    public void IlpSolverWithoutWarmStartAndAttributesDoesNotLogWarning() =>
        Assert.DoesNotContain(SolveWithScipAndLogger(withWarmStart: false).Logger.Entries,
            entry => entry.Level == LogLevel.Warning);

    [Fact]
    public void IlpSolverWithVariableAttributesReturnsOptimalResult()
    {
        var (result, _, x, y) = SolveWithScipAndLogger(withWarmStart: false, withAttributes: true);

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithVariableAttributesLogsWarningThatAttributesAreIgnored() =>
        Assert.Contains(SolveWithScipAndLogger(withWarmStart: false, withAttributes: true).Logger.Entries,
            entry => entry.Level == LogLevel.Warning && entry.Message.Contains("ignoring 2 attributes"));

    [Fact]
    public void IlpSolverWithHiGhsAndWarmStartReturnsOptimalResult()
    {
        var (result, _, x, y) = SolveWithHiGhsAndWarmStart();

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void IlpSolverWithHiGhsLogsWarningThatWarmStartIsIgnored() =>
        Assert.Contains(SolveWithHiGhsAndWarmStart().Logger.Entries,
            entry => entry.Level == LogLevel.Warning && entry.Message.Contains(IgnoredWarmStartMessage));

    [Fact]
    public void IlpSolverWithHiGhsDoesNotForwardWarmStart() =>
        Assert.DoesNotContain(SolveWithHiGhsAndWarmStart().Logger.Entries,
            entry => entry.Message.Contains("Setting solution hint"));

    [Fact]
    public void IlpSolverWithHiGhsAsFallbackReturnsOptimalObjectiveValue() =>
        Assert.Equal(new ObjectiveValue(2), SolveWithHiGhsAsFallbackAndWarmStart().Result.ObjectiveValue);

    [Fact]
    public void IlpSolverWithHiGhsAsFallbackSwitchesSolver() =>
        Assert.True(SolveWithHiGhsAsFallbackAndWarmStart().Result.SwitchedToDefaultSolver);

    [Fact]
    public void IlpSolverWithHiGhsAsFallbackLogsWarningThatWarmStartIsIgnored() =>
        Assert.Contains(SolveWithHiGhsAsFallbackAndWarmStart().Logger.Entries,
            entry => entry.Level == LogLevel.Warning && entry.Message.Contains(IgnoredWarmStartMessage));

    [Fact]
    public void WarmStartFromPreviousSolutionContainsAllVariables() =>
        Assert.Equal(2, WarmStartFromPreviousSolution(out _, out _).Count);

    [Fact]
    public void WarmStartFromPreviousSolutionEqualsOptimalSolution()
    {
        var warmStart = WarmStartFromPreviousSolution(out var x, out var y);

        Assert.Equal(
            new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new RealScalar(0)).Add(y, new RealScalar(1)),
            warmStart);
    }

    [Fact]
    public void ResolvingWithPreviousSolutionAsWarmStartReturnsOptimalResult()
    {
        var warmStart = WarmStartFromPreviousSolution(out var x, out var y);
        var model = CreateIlpModel(out _, out _).WithWarmStart(warmStart);

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        Assert.Equal(OptimalIlpResult(x, y), result);
    }

    [Fact]
    public void CpSolverWithSuboptimalWarmStartReturnsOptimalResult()
    {
        var model = new OptimizationModel<IIntegerVariable<IIntegerScalar>, IIntegerScalar, IIntegerScalar>();
        var x = model.NewBinaryVariable<BinaryVariable>("x");
        var y = model.NewBinaryVariable<BinaryVariable>("y");
        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).AddTermToSum(new IntegerScalar(1), y)
            .Build(new IntegralInterval(0, 1)));
        var completedModel = model.SetObjective(model.CreateObjectiveFunctionBuilder()
                .AddTermToSum(new IntegerScalar(1), x).AddTermToSum(new IntegerScalar(2), y).Build())
            .WithWarmStart(new WarmStart<IIntegerVariable<IIntegerScalar>, IIntegerScalar>()
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
    public void LpSolverWithWarmStartReturnsOptimalResult(LpSolverType solverType)
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
            .WithWarmStart(new WarmStart<ContinuousVariable<IRealScalar>, IRealScalar>()
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
    public void GurobiNativeSolverWithSolutionLimitReturnsObjectiveValueOfWarmStart() =>
        Assert.Equal(new ObjectiveValue(1), SolveWithGurobiAndSolutionLimit().Result.ObjectiveValue);

    [RequiresGurobiLicenceFact]
    public void GurobiNativeSolverWithSolutionLimitReturnsFeasibleStatus() =>
        Assert.Equal(SolverResultStatus.Feasible, SolveWithGurobiAndSolutionLimit().Result.SolverResultStatus);

    [RequiresGurobiLicenceFact]
    public void GurobiNativeSolverWithSolutionLimitReturnsWarmStartValues()
    {
        var (result, x, y) = SolveWithGurobiAndSolutionLimit();

        Assert.Equal(
            SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                (x, new RealScalar(1)), (y, new RealScalar(0))),
            result.SolutionValues);
    }

    [RequiresGurobiLicenceFact]
    public void GurobiNativeSolverWithWarmStartAndAttributesReturnsOptimalResult()
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
    public void GurobiNativeSolverWithoutLicenceForwardsWarmStartToFallbackSolver() =>
        Assert.Contains(SolveWithGurobiFallingBackToScip().Logger.Entries,
            entry => entry.Message.Contains(SolutionHintMessage));

    [RequiresNoGurobiLicenceFact]
    public void GurobiNativeSolverWithoutLicenceLogsThatAttributesAreIgnoredByFallbackSolver() =>
        Assert.Contains(SolveWithGurobiFallingBackToScip().Logger.Entries,
            entry => entry.Message.Contains("ignoring 1 attributes"));

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result, FakeLogger Logger,
        IIntegerVariable<IRealScalar> X, IIntegerVariable<IRealScalar> Y) SolveWithScipAndLogger(
        bool withWarmStart, bool withAttributes = false)
    {
        var logger = new FakeLogger();
        var model = CreateIlpModel(out var x, out var y);
        if (withWarmStart) model = model.WithWarmStart(SuboptimalWarmStart(x, y));
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
        IIntegerVariable<IRealScalar> X, IIntegerVariable<IRealScalar> Y) SolveWithHiGhsAndWarmStart()
    {
        var logger = new FakeLogger();
        var model = CreateIlpModel(out var x, out var y).WithWarmStart(SuboptimalWarmStart(x, y));

        var result = new IlpSolver(IlpSolverType.HiGhs, logger: logger).Solve(model, SilentParameter());

        return (result, logger, x, y);
    }

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result, FakeLogger Logger)
        SolveWithHiGhsAsFallbackAndWarmStart()
    {
        var logger = new FakeLogger();
        var model = CreateIlpModel(out var x, out var y).WithWarmStart(SuboptimalWarmStart(x, y));

        var result = new IlpSolver((IlpSolverType)int.MaxValue, IlpSolverType.HiGhs, logger)
            .Solve(model, SilentParameter());

        return (result, logger);
    }

    private static WarmStart<IIntegerVariable<IRealScalar>, IRealScalar> WarmStartFromPreviousSolution(
        out IIntegerVariable<IRealScalar> x, out IIntegerVariable<IRealScalar> y)
    {
        var model = CreateIlpModel(out x, out y);

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(model, SilentParameter());

        return ((SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>)result.SolutionValues)
            .ToWarmStart();
    }

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result,
        IIntegerVariable<IRealScalar> X, IIntegerVariable<IRealScalar> Y) SolveWithGurobiAndSolutionLimit()
    {
        var model = CreateIlpModel(out var x, out var y).WithWarmStart(SuboptimalWarmStart(x, y));

        var result = new GurobiNativeSolver().Solve(model, new SolverParameter(EnableSolverOutput.False,
            AdditionalSolverSpecificParameters: [("SolutionLimit", "1"), ("Presolve", "0")]));

        return (result, x, y);
    }

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result, FakeLogger Logger,
        IIntegerVariable<IRealScalar> X, IIntegerVariable<IRealScalar> Y) SolveWithGurobiAndAttributes()
    {
        var logger = new FakeLogger();
        var model = CreateIlpModel(out var x, out var y)
            .WithWarmStart(SuboptimalWarmStart(x, y))
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
            .WithWarmStart(SuboptimalWarmStart(x, y))
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

    private static WarmStart<IIntegerVariable<IRealScalar>, IRealScalar> SuboptimalWarmStart(
        IIntegerVariable<IRealScalar> x, IIntegerVariable<IRealScalar> y) =>
        new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, BinaryScalar.One).Add(y, BinaryScalar.Zero);

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
