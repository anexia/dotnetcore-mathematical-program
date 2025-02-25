// ------------------------------------------------------------------------------------------
//  <copyright file = "ConstraintBuilderTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using static Anexia.MathematicalProgram.Tests.Factory.ConstraintFactory;
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.WeightedSumFactory;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class ConstraintBuilderTest
{
    [Fact]
    public void ConstraintBuilderAddTermReturnsCorrectResult()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var v2 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 2), "v2");
        var v3 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 3), "v3");
        
        var constraint = model.CreateConstraintBuilder()
            .AddTermToSum(1, v1)
            .AddTermToSum(2, v2)
            .AddTermToSum(-3, v3)
            .Build(new IntegralInterval(-10, 20));


        Assert.Equal(
            Constraint(WeightedSum((v1, 1), (v2, 2), (v3, -3)), Interval(-10, 20)),
            constraint);
    }

    [Fact]
    public void ConstraintBuilderAddWeightedSumReturnsCorrectResult()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var v2 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 2), "v2");
        var v3 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 3), "v3");

        var constraint = model.CreateConstraintBuilder()
            .AddWeightedSum(
                [v1, v2, v3], [1, 2, -3])
            .Build(new IntegralInterval(-10, 20));

        Assert.Equal(
            Constraint(WeightedSum((v1, 1), (v2, 2), (v3, -3)), Interval(-10, 20)),
            constraint);
    }

    [Fact]
    public void AddWeightedSumThrowsCorrectException()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var v2 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 2), "v2");
        var v3 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 3), "v3");

        var exception = Assert.Throws<NumberOfWeightsNotEqualToNumberOfVariablesException>(() =>
            model.CreateConstraintBuilder()
                .AddWeightedSum(model.CreateWeightedSumBuilder().AddWeightedSum(
                    [v1, v2, v3], [1, 2]).Build()));

        Assert.Equal(
            "Number of weights must match number of variables. Weights: 2, Variables: 3",
            exception.Message);
    }
}