// ------------------------------------------------------------------------------------------
//  <copyright file = "LinearSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
using static System.Double;
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolutionValuesFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolverResultFactory;


namespace Anexia.MathematicalProgram.Tests.Solve;

public sealed class LinearSolverTest
{
    [Fact]
    public void SolverWithSimpleFeasibleMinimizationLpModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=2, x in (0,3)
         */

        var model = new OptimizationModel<ContinuousVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<ContinuousVariable<IRealScalar>>(Interval(0, 3), "TestVariable");

        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).Build(Point(2d)));

        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x).Build(false));

        var result = SolverFactory.SolverFor(LpSolverType.Glop).Solve(optimizationModel,
            new SolverParameter());

        Assert.Equal(
            SolverResult(
                SolutionValues<ContinuousVariable<IRealScalar>, RealScalar, IRealScalar>(
                    (x, new RealScalar(2))), new ObjectiveValue(4), new IsFeasible(true),
                new IsOptimal(true), null,
                SolverResultStatus.Optimal, false), result);
    }

    [Fact]
    public void SolverWithSimpleFeasibleMaximizationLpModelReturnsCorrectResult()
    {
        /*
         * max 2x, s.t. x<=2, x in (0,3)
         */

        var model = new OptimizationModel<ContinuousVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<ContinuousVariable<IRealScalar>>(Interval(0, 3), "TestVariable");

        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).Build(Interval(NegativeInfinity, 2)));

        var optimizationModel =
            model.SetObjective(model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x)
                .Build());

        var result = SolverFactory.SolverFor(LpSolverType.Glop).Solve(optimizationModel,
            new SolverParameter());

        Assert.Equal(
            SolverResult(
                SolutionValues<ContinuousVariable<IRealScalar>, RealScalar, IRealScalar>(
                    (x, new RealScalar(2))), new ObjectiveValue(4), new IsFeasible(true),
                new IsOptimal(true), null,
                SolverResultStatus.Optimal, false), result);
    }

    [Fact]
    public void SolverFromModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=1, x binary
         */

        var model =
            new OptimizationModel<ContinuousVariable<IRealScalar>, RealScalar, IRealScalar>();
        var v1 = model.NewVariable<ContinuousVariable<IRealScalar>>(Interval(1d, 1d), "TestVariable");

        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new RealScalar(2), v1).Build(false));

        var result = SolverFactory.SolverFor(LpSolverType.Scip).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true), ExportModelFilePath: "model.txt"));

        var resultFromModel = new LpSolver(LpSolverType.Scip).Solve(
            new ModelAsMpsFormat(File.ReadAllText("model.txt")), new SolverParameter(new EnableSolverOutput(true)));

        Assert.Equal(result, resultFromModel);
    }

    [Fact]
    public void SolverWithInfeasibleLpModelReturnsCorrectResult()
    {
        /*
         * max 2x, s.t. x=3, x in (0,1)
         */

        var model = new OptimizationModel<ContinuousVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<ContinuousVariable<IRealScalar>>(Interval(0, 1), "TestVariable");

        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).Build(Point(3d)));

        var optimizationModel =
            model.SetObjective(model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x)
                .Build());
        var result = SolverFactory.SolverFor(LpSolverType.Glop).Solve(optimizationModel, new SolverParameter());

        Assert.Equal(
            SolverResult(
                new SolutionValues<ContinuousVariable<IRealScalar>, RealScalar, IRealScalar>(
                    ReadOnlyDictionary<ContinuousVariable<IRealScalar>, RealScalar>.Empty), null,
                new IsFeasible(false),
                new IsOptimal(false), null,
                SolverResultStatus.Infeasible, false), result);
    }
}