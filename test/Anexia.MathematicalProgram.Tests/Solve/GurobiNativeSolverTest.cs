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
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolutionValuesFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolverResultFactory;


namespace Anexia.MathematicalProgram.Tests.Solve;

public sealed class GurobiNativeSolverTest
{
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
}