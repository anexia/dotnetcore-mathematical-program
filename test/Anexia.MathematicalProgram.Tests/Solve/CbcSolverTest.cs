// ------------------------------------------------------------------------------------------
//  <copyright file = "CbcSolverTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
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
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolutionValuesFactory;
using static Anexia.MathematicalProgram.Tests.Factory.SolverResultFactory;

namespace Anexia.MathematicalProgram.Tests.Solve;

public sealed class CbcSolverTest
{
    [Fact]
    [Obsolete("Obsolete")]
    public void SolverWithSimpleFeasibleIlpModelReturnsCorrectResult()
    {
        /*
         * min 2x, s.t. x=1, x integer
         */

        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(1, 1), "TestVariable");


        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), v1).Build(false));

        var result = SolverFactory.SolverFor(IlpSolverType.CbcIntegerProgramming).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));

        Assert.Equal(
            SolverResult(
                SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>((v1, new RealScalar(1))),
                new ObjectiveValue(2), new IsFeasible(true), new IsOptimal(true),
                new OptimalityGap(0), SolverResultStatus.Optimal, false), result);
    }

    [Fact]
    [Obsolete("Obsolete")]
    public void SolverWithSimpleFeasibleIlpModelWithBinaryReturnsCorrectResult()
    {
        /*
         * max 2x,x binary
         */

        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewBinaryVariable<BinaryVariable>("TestVariable");


        var optimizationModel =
            model.SetObjective(
                model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x).Build());

        var result = SolverFactory.SolverFor(IlpSolverType.CbcIntegerProgramming).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));

        Assert.Equal(
            SolverResult(
                SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>((x, new RealScalar(1))),
                new ObjectiveValue(2), new IsFeasible(true), new IsOptimal(true),
                new OptimalityGap(0), SolverResultStatus.Optimal, false), result);
    }

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

        var result = SolverFactory.SolverFor(IlpSolverType.GurobiIntegerProgramming).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));

        Assert.Equal(
            SolverResult(
                SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>((v1, new RealScalar(1))),
                new ObjectiveValue(2), new IsFeasible(true), new IsOptimal(true),
                new OptimalityGap(0), SolverResultStatus.Optimal, true), result);
    }


    [Fact]
    [Obsolete("Obsolete")]
    public void SolverWithInfeasibleIlModelReturnsCorrectResult()
    {
        //
        // max 2x, s.t. x=3, x binary
        //


        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        var x = model.NewVariable<IntegerVariable<IRealScalar>>(Interval(0, 1), "c");


        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x).Build(Point(3)));


        var optimizationModel =
            model.SetObjective(model.CreateObjectiveFunctionBuilder().AddTermToSum(new IntegerScalar(2), x)
                .Build());


        var result = SolverFactory.SolverFor(IlpSolverType.CbcIntegerProgramming).Solve(optimizationModel,
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
}