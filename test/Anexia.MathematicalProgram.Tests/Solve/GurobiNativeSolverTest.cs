// ------------------------------------------------------------------------------------------
//  <copyright file = "IlpSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using System.Collections.ObjectModel;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
using Microsoft.Extensions.Logging;
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolutionValuesFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolverResultFactory;


namespace Anexia.MathematicalProgram.Tests.Solve;

public sealed class GurobiNativeSolverTest
{
    [Fact]
    public void SolveWrapsGurobiSetupExceptionAndLogsError()
    {
        var logger = new FakeLogger();

        var exception = Assert.Throws<MathematicalProgramException>(() =>
            new GurobiNativeSolver(logger).Solve(
                CreateSimpleModel(),
                CreateInvalidGurobiParameter()));

        Assert.Contains("Error in solver:", exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.Equal(LogLevel.Error, logger.LastLogLevel);
        Assert.Same(exception.InnerException, logger.LastException);
    }

    [Fact]
    public void SolveWrapsGurobiSetupExceptionWithoutLogger()
    {
        var exception = Assert.Throws<MathematicalProgramException>(() =>
            new GurobiNativeSolver().Solve(
                CreateSimpleModel(),
                CreateInvalidGurobiParameter()));

        Assert.Contains("Error in solver:", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact(Skip = "Licence needed")]
    public void SolverWithSimpleFeasibleIlpModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=1, x binary
         */

        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(1, 1), "TestVariable");


        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), v1).Build(false));

        var result = new IlpSolver(IlpSolverType.GurobiIntegerProgramming).SolveWithoutORTools(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));

        Assert.Equal(
            SolverResult(
                SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    (v1, new RealScalar(1))), new ObjectiveValue(2), new IsFeasible(true),
                new IsOptimal(true), new OptimalityGap(0),
                SolverResultStatus.Optimal, false), result);
    }

    [Fact(Skip = "Licence needed")]
    public void SolverWithSimpleFeasibleBinaryIlpModelReturnsCorrectResult()
    {
        /*
         * max 2x, x binary
         */

        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var v1 = model.NewBinaryVariable<BinaryVariable>("TestVariable");

        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), v1).Build());

        var result = new IlpSolver(IlpSolverType.GurobiIntegerProgramming).SolveWithoutORTools(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));

        Assert.Equal(
            SolverResult(
                SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    (v1, new RealScalar(1))), new ObjectiveValue(2), new IsFeasible(true),
                new IsOptimal(true), new OptimalityGap(0),
                SolverResultStatus.Optimal, false), result);
    }

    [Fact(Skip = "Licence needed")]
    public void SolverWithInfeasibleIlModelReturnsCorrectResult()
    {
        /*
         * max 2x, s.t. x=3, x binary
         */

        var model =
            new OptimizationModel<IntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(0, 1), "c");


        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).Build(Point(3)));


        var optimizationModel =
            model.SetObjective(model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x)
                .Build());


        var result = new IlpSolver(IlpSolverType.GurobiIntegerProgramming).SolveWithoutORTools(optimizationModel,
            new SolverParameter(
                EnableSolverOutput.True,
                RelativeGap.EMinus7,
                null,
                new NumberOfThreads(2)));


        Assert.Equal(
            SolverResult(
                new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    ReadOnlyDictionary<IIntegerVariable<IRealScalar>, RealScalar>.Empty), null, new IsFeasible(false),
                new IsOptimal(false), null,
                SolverResultStatus.Infeasible, false), result);
    }

    [Fact(Skip = "Licence needed")]
    public void SolverWithUnboundedIlModelReturnsCorrectResult()
    {
        /*
         * max 2x, x positive
         */

        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(0, double.PositiveInfinity), "x");

        var optimizationModel =
            model.SetObjective(model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x)
                .Build());

        var result = new IlpSolver(IlpSolverType.GurobiIntegerProgramming).SolveWithoutORTools(optimizationModel,
            new SolverParameter());

        Assert.Equal(
            SolverResult(
                new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    ReadOnlyDictionary<IIntegerVariable<IRealScalar>, RealScalar>.Empty), null, new IsFeasible(false),
                new IsOptimal(false), null,
                SolverResultStatus.Unbounded, false), result);
    }

    [Fact(Skip = "Licence needed")]
    public void GurobiWithoutORToolsGivesSameResultAsWithORTools()
    {
        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var x = model.NewVariable<IntegerVariable<IRealScalar>>(
            new IntegralInterval(new IntegerScalar(1), new IntegerScalar(3)), "x");
        var y = model.NewVariable<IntegerVariable<IRealScalar>>(
            new IntegralInterval(0, 1), "y");
        var xMinusY = model.CreateWeightedSumBuilder()
            .AddWeightedSum([x, y], [1, -1]).Build();

        var constraint = model.CreateConstraintBuilder()
            .AddWeightedSum(xMinusY)
            .Build(new RealInterval(0, double.PositiveInfinity));

        model.AddConstraint(constraint);

        var objFunction = model.CreateObjectiveFunctionBuilder().AddTermToSum(2, x)
            .AddTermToSum(2, y).Build(false);

        var optimizationModel = model.SetObjective(objFunction);

        var resultORTools = SolverFactory.SolverFor(IlpSolverType.GurobiIntegerProgramming).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(false), RelativeGap.EMinus7,
                new TimeLimitInMilliseconds(10000), new NumberOfThreads(2), AdditionalSolverSpecificParameters:
                [
                    ("ResultFile", "resultOR.sol")
                ]));


        var resultGurobiAPI = new IlpSolver(IlpSolverType.GurobiIntegerProgramming).SolveWithoutORTools(
            optimizationModel,
            new SolverParameter(new EnableSolverOutput(false), RelativeGap.EMinus7,
                new TimeLimitInMilliseconds(10000), new NumberOfThreads(2),
                AdditionalSolverSpecificParameters: [("ResultFile", "resultGRB.sol")]));

        Assert.Equal(resultORTools, resultGurobiAPI);
    }

    private sealed class FakeLogger : ILogger<IlpSolver>
    {
        internal LogLevel? LastLogLevel { get; private set; }

        internal Exception? LastException { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            LastLogLevel = logLevel;
            LastException = exception;
        }
    }

    private static ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>
        CreateSimpleModel()
    {
        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(0, 1), "TestVariable");

        return model.SetObjective(
            model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(1), v1).Build(false));
    }

    private static SolverParameter CreateInvalidGurobiParameter() =>
        new(
            EnableSolverOutput.False,
            AdditionalSolverSpecificParameters: [("__invalid_gurobi_parameter__", "1")]);
}
