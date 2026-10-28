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
using Gurobi;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolutionValuesFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolverResultFactory;


namespace Anexia.MathematicalProgram.Tests.Solve;

public sealed class IlpSolverTest
{
    [Fact]
    public void SolveWithGurobiNativeWrapsNonLicenceGurobiExceptionMessage()
    {
        var (exception, _) = SolveWithInvalidGurobiParameter();

        Assert.Contains("Error in solver:", exception.Message);
    }

    [Fact]
    public void SolveWithGurobiNativeKeepsGurobiExceptionAsInnerException()
    {
        var (exception, _) = SolveWithInvalidGurobiParameter();

        Assert.IsType<GRBException>(exception.InnerException);
    }

    [Fact]
    public void SolveWithGurobiNativeDoesNotFallBackOnNonLicenceGurobiException()
    {
        var (_, logger) = SolveWithInvalidGurobiParameter();

        Assert.DoesNotContain(logger.Messages,
            message => message.Contains("No Gurobi licence found", StringComparison.Ordinal));
    }

    [Fact]
    public void SolveWithUnsupportedSolverReturnsResultFromFallbackSolver()
    {
        var (result, _, variable) = SolveWithUnsupportedSolver();

        Assert.Equal(
            SolverResult(
                SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    (variable, new RealScalar(1))),
                new ObjectiveValue(2),
                new IsFeasible(true),
                new IsOptimal(true),
                new OptimalityGap(0),
                SolverResultStatus.Optimal,
                true),
            result);
    }

    [Fact]
    public void SolveWithUnsupportedSolverLogsSwitchToFallbackSolver()
    {
        var (_, logger, _) = SolveWithUnsupportedSolver();

        Assert.Contains(logger.Messages,
            message => message.Contains("switching to fallback solver", StringComparison.Ordinal));
    }

    [Fact]
    public void SolveModelAsMpsFormatThrowsWhenSolverAndFallbackAreUnsupported()
    {
        var exception = Assert.Throws<SolverNotSupportedException>(() =>
            new IlpSolver((IlpSolverType)int.MaxValue, (IlpSolverType)int.MaxValue)
                .Solve(new ModelAsMpsFormat(string.Empty), new SolverParameter()));

        Assert.Equal(
            "Neither the expected solver 2147483647 nor fallback solver 2147483647 could be initialized.",
            exception.Message);
    }

    [Fact]
    public void SolveWithExportModelFilePathWritesExportFile()
    {
        var roundTrip = SolveExportedModelWithDefaultOverload();

        Assert.True(roundTrip.ExportFileExisted);
    }
    
    [Fact]
    public void SolveModelAsMpsFormatDefaultOverloadReturnsSameObjectiveValue()
    {
        var roundTrip = SolveExportedModelWithDefaultOverload();

        Assert.Equal(roundTrip.ExportedResult.ObjectiveValue, roundTrip.ResultFromMps.ObjectiveValue);
    }

    [Fact]
    public void SolveModelAsMpsFormatDefaultOverloadReturnsSameSolutionValue()
    {
        var roundTrip = SolveExportedModelWithDefaultOverload();

        Assert.Equal(new RealScalar(1), SingleSolutionValue(roundTrip.ResultFromMps).Value);
    }

    [Fact]
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

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));

        Assert.Equal(
            SolverResult(
                SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    (v1, new RealScalar(1))), new ObjectiveValue(2), new IsFeasible(true),
                new IsOptimal(true), new OptimalityGap(0),
                SolverResultStatus.Optimal, false), result);
    }

    [Fact]
    public void SolverFromModelWithIntegerIntervalReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=1, x  integer [0,1]
         */

        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(1d, 1d), "TestVariable");

        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x).Build(false));

        var exportFilePath = NewExportFilePath();

        try
        {
            var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(optimizationModel,
                new SolverParameter(new EnableSolverOutput(true), ExportModelFilePaths: exportFilePath));

            var resultFromModel = new IlpSolver(IlpSolverType.Scip).Solve(
                new ModelAsMpsFormat(File.ReadAllText(exportFilePath)),
                new SolverParameter(new EnableSolverOutput(true)));

            Assert.Equal(result, resultFromModel);
        }
        finally
        {
            File.Delete(exportFilePath);
        }
    }

    [Fact]
    public void SolverFromModelWithBinaryIntervalReturnsCorrectResult()
    {
        /*
         * max 2x, x integer binary interval
         */

        var model =
            new OptimizationModel<IIntegerVariable<IBinaryScalar>, IRealScalar, IBinaryScalar>();
        var x = model.NewVariable<IntegerVariable<IBinaryScalar>>(new BinaryInterval(), "TestVariable");

        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x).Build());

        var exportFilePath = NewExportFilePath();

        try
        {
            var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(optimizationModel,
                new SolverParameter(new EnableSolverOutput(true), ExportModelFilePaths: exportFilePath));

            var resultFromModel = new IlpSolver(IlpSolverType.Scip).Solve(
                new ModelAsMpsFormat(File.ReadAllText(exportFilePath)),
                new SolverParameter(new EnableSolverOutput(true)));

            Assert.Equal(result.ObjectiveValue, resultFromModel.ObjectiveValue);
        }
        finally
        {
            File.Delete(exportFilePath);
        }
    }

    [Fact]
    public void SolverFromModelWithBinaryVariableReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=1, x binary
         */

        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewBinaryVariable<BinaryVariable>("TestVariable");

        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x).Build(false));

        var exportFilePath = NewExportFilePath();

        try
        {
            var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(optimizationModel,
                new SolverParameter(new EnableSolverOutput(true), ExportModelFilePaths: exportFilePath));

            var resultFromModel = new IlpSolver(IlpSolverType.Scip).Solve(
                new ModelAsMpsFormat(File.ReadAllText(exportFilePath)),
                new SolverParameter(new EnableSolverOutput(true)));

            Assert.Equal(result, resultFromModel);
        }
        finally
        {
            File.Delete(exportFilePath);
        }
    }

    [Fact]
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


        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(optimizationModel,
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

    [Fact]
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

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(optimizationModel,
            new SolverParameter());

        Assert.Equal(UnboundedSolverResult(), result);
    }

    [Fact]
    public void SolverAdditionalLoggingDoesNotChangeSolverResult()
    {
        var (result, _) = SolveUnboundedModelWithFileLogger();

        Assert.Equal(UnboundedSolverResult(), result);
    }

    private static string NewExportFilePath() => Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.mps");

    // An invalid Gurobi parameter fails during environment setup with a GRBException that is
    // unrelated to licensing. The solver must surface it instead of silently falling back.
    private static (MathematicalProgramException Exception, FakeLogger Logger) SolveWithInvalidGurobiParameter()
    {
        var optimizationModel = CreateSimpleOptimizationModel(out _);
        var logger = new FakeLogger();

        var exception = Assert.Throws<MathematicalProgramException>(() =>
            new IlpSolver(IlpSolverType.GurobiNativeIntegerProgramming, IlpSolverType.Scip, logger)
                .Solve(
                    optimizationModel,
                    new SolverParameter(
                        EnableSolverOutput.False,
                        AdditionalSolverSpecificParameters: [("__invalid_gurobi_parameter__", "1")])));

        return (exception, logger);
    }

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result,
        FakeLogger Logger,
        IIntegerVariable<IRealScalar> Variable) SolveWithUnsupportedSolver()
    {
        var optimizationModel = CreateSimpleOptimizationModel(out var variable);
        var logger = new FakeLogger();

        var result = new IlpSolver((IlpSolverType)int.MaxValue, IlpSolverType.Scip, logger)
            .Solve(optimizationModel, new SolverParameter(EnableSolverOutput.False));

        return (result, logger, variable);
    }

    private static (bool ExportFileExisted,
        IReadOnlyList<string> LoggerMessages,
        ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> ExportedResult,
        ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> ResultFromMps,
        IIntegerVariable<IRealScalar> Variable) SolveExportedModelWithDefaultOverload()
    {
        var exportFilePath = NewExportFilePath();
        var optimizationModel = CreateSimpleOptimizationModel(out var variable);
        var logger = new FakeLogger();

        try
        {
            var exportedResult = new IlpSolver(IlpSolverType.Scip, logger: logger)
                .Solve(optimizationModel, new SolverParameter(
                    EnableSolverOutput.False,
                    ExportModelFilePaths: exportFilePath));

            var resultFromMps = new IlpSolver(IlpSolverType.Scip)
                .Solve(new ModelAsMpsFormat(File.ReadAllText(exportFilePath)));

            return (File.Exists(exportFilePath), logger.Messages, exportedResult, resultFromMps, variable);
        }
        finally
        {
            if (File.Exists(exportFilePath)) File.Delete(exportFilePath);
        }
    }

    private static KeyValuePair<IIntegerVariable<IRealScalar>, RealScalar> SingleSolutionValue(
        ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> result) =>
        ((IEnumerable<KeyValuePair<IIntegerVariable<IRealScalar>, RealScalar>>)result.SolutionValues).Single();

    private static (ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Result, string LogContent)
        SolveUnboundedModelWithFileLogger()
    {
        /*
         * max 2x, x positive
         */

        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(0, double.PositiveInfinity), "x");

        var optimizationModel =
            model.SetObjective(model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x)
                .Build());

        var logFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.log");

        try
        {
            ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> result;

            using (var loggerFactory = new SerilogLoggerFactory(
                       new LoggerConfiguration().WriteTo
                           .File(logFile, outputTemplate: "[{SourceContext}] [{Level:u4}] {Message}")
                           .CreateLogger(),
                       dispose: true))
            {
                result = SolverFactory.SolverFor(IlpSolverType.Scip, null, loggerFactory.CreateLogger<IlpSolver>())
                    .Solve(optimizationModel, new SolverParameter());
            }

            return (result, File.ReadAllText(logFile));
        }
        finally
        {
            if (File.Exists(logFile)) File.Delete(logFile);
        }
    }

    private static SolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> UnboundedSolverResult() =>
        SolverResult(
            new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                ReadOnlyDictionary<IIntegerVariable<IRealScalar>, RealScalar>.Empty), null, null,
            new IsOptimal(false), null,
            SolverResultStatus.Unbounded, false);

    private static ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>
        CreateSimpleOptimizationModel(out IIntegerVariable<IRealScalar> variable)
    {
        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        variable = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(1, 1), "TestVariable");

        return model.SetObjective(
            model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), variable).Build(false));
    }

    private sealed class FakeLogger : ILogger<IlpSolver>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
