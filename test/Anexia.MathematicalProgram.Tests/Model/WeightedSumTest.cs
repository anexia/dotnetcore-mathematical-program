// ------------------------------------------------------------------------------------------
//  <copyright file = "WeightedSumTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using static Anexia.MathematicalProgram.Tests.Factory.WeightedSumFactory;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class WeightedSumTest
{
    [Fact]
    public void AddTermsTest()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        var terms = new WeightedSum<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var v2 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 2), "v2");

        Assert.Equal(WeightedSum((v1, 1), (v2, 2)), terms
            .Add(v1, 1).Add(v2, 2));
    }

    [Fact]
    public void TermsWithDifferentRepresentationAreEqual()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        var sum1 = new WeightedSum<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        var sum2 = new WeightedSum<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var v2 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 2), "v2");
        var v3 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(-1, 5), "v3");

        Assert.True(
            sum1.Add(v1, 1)
                .Add(v2, 2)
                .Add(v3, 3)
                .Equals(sum2
                    .Add(v2, 1)
                    .Add(v3, 2)
                    .Add(v2, 1)
                    .Add(v1, 1)
                    .Add(v3, 1)
                ));
    }

    [Fact]
    public void ObjectiveFunctionBuilderReturnsCorrectResult()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var v2 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 2), "v2");


        Assert.Equal(
            new ObjectiveFunction<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(3,
                WeightedSum((v1, 1), (v2, 2)), true),
            model.CreateObjectiveFunctionBuilder()
                .AddTermToSum(1, v1).AddTermToSum(2, v2)
                .Build(true, 3));
    }
}