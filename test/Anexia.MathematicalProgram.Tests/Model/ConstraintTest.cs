// ------------------------------------------------------------------------------------------
//  <copyright file = "ConstraintTest.cs" company = "ANEXIA Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using static Anexia.MathematicalProgram.Tests.Factory.WeightedSumFactory;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class ConstraintTest
{
    [Fact]
    public void ConstraintKeepsWeightedSumIntervalAndName()
    {
        var (weightedSum, interval) = CreateWeightedSumAndInterval();

        var constraint = new Constraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            weightedSum,
            interval,
            "capacity");

        Assert.Same(weightedSum, constraint.WeightedSum);
        Assert.Same(interval, constraint.Interval);
        Assert.Equal("capacity", constraint.Name);
    }

    [Fact]
    public void ConstraintsWithEquivalentMembersAreEqual()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        var variable = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 10), "x");

        var first = new Constraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            WeightedSum((variable, 2)),
            new RealInterval(-4, 8),
            "limit");
        var second = new Constraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            WeightedSum((variable, 2)),
            new RealInterval(-4, 8),
            "limit");
        var differentName = new Constraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            WeightedSum((variable, 2)),
            new RealInterval(-4, 8),
            "other-limit");

        Assert.Equal(first, second);
        Assert.NotEqual(first, differentName);
    }

    [Fact]
    public void DefaultConstraintsCollectionIsEmpty()
    {
        var constraints = new Constraints<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        Assert.True(constraints.Count is 0);
        Assert.Empty(constraints);
        Assert.Empty(((IEnumerable)constraints).Cast<object>());
    }

    [Fact]
    public void ConstraintsConstructorMaterializesEnumerable()
    {
        var constraint = CreateConstraint("c1");
        var source = new List<IConstraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>> { constraint };
        var constraints = new Constraints<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            source.Where(_ => true));

        source.Clear();

        Assert.True(constraints.Count is 1);
        Assert.Equal(constraint, Assert.Single(constraints));
    }

    [Fact]
    public void AddReturnsNewConstraintsCollectionWithoutChangingOriginal()
    {
        var first = CreateConstraint("c1");
        var second = CreateConstraint("c2");
        var constraints = new Constraints<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(first);

        var updated = constraints.Add(second);

        Assert.True(constraints.Count is 1);
        Assert.Equal(first, Assert.Single(constraints));
        Assert.True(updated.Count is 2);
        Assert.Collection(
            updated,
            constraint => Assert.Equal(first, constraint),
            constraint => Assert.Equal(second, constraint));
    }

    private static Constraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> CreateConstraint(string name)
    {
        var (weightedSum, interval) = CreateWeightedSumAndInterval();

        return new Constraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            weightedSum,
            interval,
            name);
    }

    private static (IWeightedSum<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> WeightedSum,
        IInterval<IRealScalar> Interval) CreateWeightedSumAndInterval()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        var variable = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 10), "x");

        return (WeightedSum((variable, 2)), new RealInterval(-4, 8));
    }
}
