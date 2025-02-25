// ------------------------------------------------------------------------------------------
//  <copyright file = "ConstantTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using static Anexia.MathematicalProgram.Tests.Factory.ConstraintFactory;
using static Anexia.MathematicalProgram.Tests.Factory.IntervalFactory;
using static Anexia.MathematicalProgram.Tests.Factory.WeightedSumFactory;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class ConstantTest
{
    [Fact]
    public void AddConstraintReturnsCorrectException()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var v1 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var v2 = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 2), "v2");

        var constraint = model.CreateConstraintBuilder()
            .AddTermToSum(1, v1)
            .AddTermToSum(2, v2)
            .AddTermToSum(2, v1)
            .Build(new IntegralInterval(-10, 20));

        var constraints = new Constraints<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        constraints = constraints.Add(constraint);


        Assert.Equal(
            Constraints(Constraint(WeightedSum((v1, 3), (v2, 2)), Interval(-10, 20))),
            constraints);
    }
}