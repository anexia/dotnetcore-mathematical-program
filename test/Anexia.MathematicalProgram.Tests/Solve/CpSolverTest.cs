// ------------------------------------------------------------------------------------------
//  <copyright file = "CpSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using System.Collections.Immutable;
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

public sealed class CpSolverTest
{
    [Fact]
    public void SolverWithSimpleFeasibleCpModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=1, x binary
         */

        var model =
            new OptimizationModel<IIntegerVariable<IIntegerScalar>, IIntegerScalar, IIntegerScalar>();
        var v1 = model.NewVariable<IntegerVariable<IIntegerScalar>>(Interval(1, 1), "TestVariable");

        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), v1).Build(false));

        var result = SolverFactory.NewCpSolver().Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));

        Assert.Equal(
            SolverResult(
                SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(
                    (v1, new IntegerScalar(1))), new ObjectiveValue(2), new IsFeasible(true),
                new IsOptimal(true), new OptimalityGap(0),
                SolverResultStatus.Optimal, false), result);
    }

    [Fact]
    public void SolverWithInfeasibleIlModelReturnsCorrectResult()
    {
        //
        // max 2x, s.t. x=3, x binary
        //

        var model =
            new OptimizationModel<IIntegerVariable<IIntegerScalar>, IIntegerScalar, IIntegerScalar>();
        var x = model.NewVariable<IntegerVariable<IIntegerScalar>>(Interval(0, 1), "c");


        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).Build(new IntegralInterval(3, 3)));


        var optimizationModel =
            model.SetObjective(model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x)
                .Build());

        var result = SolverFactory.NewCpSolver().Solve(optimizationModel,
            new SolverParameter(
                EnableSolverOutput.True,
                RelativeGap.EMinus7,
                null,
                new NumberOfThreads(2)));

        Assert.Equal(
            SolverResult(
                new SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(
                    ReadOnlyDictionary<IIntegerVariable<IIntegerScalar>, IntegerScalar>.Empty), null,
                new IsFeasible(false),
                new IsOptimal(false), null,
                SolverResultStatus.Infeasible, false), result);
    }


    [Fact]
    public void CpEnumeratingAllSolutionsWorksAsExpected()
    {
        /*
         * min 2x, s.t. x=1, x binary
         */

        var model =
            new OptimizationModel<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>();

        var x = model.NewVariable<IntegerVariable<IIntegerScalar>>(
            new IntegralInterval(0, 1), "x");
        var y = model.NewVariable<IntegerVariable<IIntegerScalar>>(
            new IntegralInterval(0, 1), "y");
        var constraint = model.CreateConstraintBuilder().AddTermToSum(1, x)
            .AddTermToSum(1, y).Build(new IntegralInterval(0, 3));

        model.AddConstraint(constraint);
        var objFunction = model.CreateObjectiveFunctionBuilder().AddTermToSum(2, x)
            .AddTermToSum(2, y).Build();

        var testSolutionCallBack = new TestSolutionCallBack();

        _ = new ConstraintProgrammingSolver().Solve(model.SetObjective(objFunction),
            new SolverParameter(new EnableSolverOutput(true)),
            testSolutionCallBack, false);

        Assert.Equal([
            SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(
                (x, 0),
                (y, 0)),
            SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(
                (x, 0),
                (y, 1)),
            SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(
                (x, 1),
                (y, 0)),
            SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(
                (x, 1),
                (y, 1))
        ], testSolutionCallBack.Solutions.ToHashSet());
    }
}

public class TestSolutionCallBack : ICpSolutionCallback
{
    public List<SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>> Solutions { get; } =
        new();

    public void OnSolutionCallback(
        SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar> solutionValues)
    {
        Console.WriteLine(solutionValues);
        Solutions.Add(solutionValues);
    }
}