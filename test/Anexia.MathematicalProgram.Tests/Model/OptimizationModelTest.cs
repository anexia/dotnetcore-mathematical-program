// ------------------------------------------------------------------------------------------
//  <copyright file = "OptimizationModelTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class OptimizationModelTest
{
    [Fact]
    public void NewVariableThrowsExceptionOnDuplicateNamesTest()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        _ = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");

        Assert.Throws<VariableAlreadyExistsException<IRealScalar>>(() =>
            model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1"));
    }

    [Fact]
    public void OptimizationModelsWithSameVariablesAndConstraintsAreEqualTest()
    {
        var model1 = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        var model2 = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var m1V1 = model1.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var m1V2 = model1.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 2), "v2");

        var m2V1 = model2.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var m2V2 = model2.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 2), "v2");

        var constraintM1 = model1.CreateConstraintBuilder()
            .AddTermToSum(1, m1V1)
            .AddTermToSum(2, m1V2)
            .Build(new IntegralInterval(-10, 20));

        var constraintM2 = model1.CreateConstraintBuilder()
            .AddTermToSum(1, m2V1)
            .AddTermToSum(2, m2V2)
            .Build(new IntegralInterval(-10, 20));

        model1.AddConstraint(constraintM1);
        model2.AddConstraint(constraintM2);

        Assert.True(model1.Equals(model2));
    }
}