// ------------------------------------------------------------------------------------------
//  <copyright file = "IlpSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using System.Collections.ObjectModel;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
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
    public void SolverFromModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=1, x binary
         */

        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(1d, 1d), "TestVariable");

        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), v1).Build(false));

        var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true), ExportModelFilePath: "model.txt"));

        var resultFromModel = new IlpSolver(IlpSolverType.Scip).Solve(
            new ModelAsMpsFormat(File.ReadAllText("model.txt")), new SolverParameter(new EnableSolverOutput(true)));

        Assert.Equal(result, resultFromModel);
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

        Assert.Equal(
            SolverResult(
                new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    ReadOnlyDictionary<IIntegerVariable<IRealScalar>, RealScalar>.Empty), null, new IsFeasible(false),
                new IsOptimal(false), null,
                SolverResultStatus.Unbounded, false), result);
    }

    [Fact]
    public void SolverAdditionalLoggingWorks()
    {
        /*
         * max 2x, x positive
         */

        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(0, double.PositiveInfinity), "x");

        var optimizationModel =
            model.SetObjective(model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x)
                .Build());

        var logFile = "tmp.log";

        var result = SolverFactory.SolverFor(IlpSolverType.Scip, null,
            new SerilogLoggerFactory(new LoggerConfiguration().WriteTo
                    .File(logFile, outputTemplate: "[{SourceContext}] [{Level:u4}] {Message}").CreateLogger())
                .CreateLogger<IlpSolver>()).Solve(
            optimizationModel,
            new SolverParameter());

        using (var streamReader = new StreamReader(logFile))
        {
            Assert.Equal(
                "[Anexia.MathematicalProgram.Solve.IlpSolver] [INFO] Initialized Solver Scip with TimeLimit: \"unbounded\" and solver specific parameters \"\"",
                streamReader.ReadToEnd());
        }

        File.Delete(logFile);

        Assert.Equal(
            SolverResult(
                new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    ReadOnlyDictionary<IIntegerVariable<IRealScalar>, RealScalar>.Empty), null, new IsFeasible(false),
                new IsOptimal(false), null,
                SolverResultStatus.Unbounded, false), result);
    }
}