// ------------------------------------------------------------------------------------------
//  <copyright file = "CompletedOptimizationModelHintTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Hint;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using static Anexia.MathematicalProgram.Tests.Factory.SolutionValuesFactory;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class CompletedOptimizationModelHintTest
{
    [Fact]
    public void CompletedModelHasNoWarmStartByDefault() => Assert.Null(CreateModel(out _, out _).WarmStart);

    [Fact]
    public void CompletedModelHasNoVariableAttributesByDefault() =>
        Assert.Null(CreateModel(out _, out _).VariableAttributes);

    [Fact]
    public void WithWarmStartSetsWarmStart()
    {
        var model = CreateModel(out var x, out var y);
        var warmStart = WarmStartFor(x, y);

        Assert.Equal(warmStart, model.WithWarmStart(warmStart).WarmStart);
    }

    [Fact]
    public void WithWarmStartLeavesVariableAttributesUnset()
    {
        var model = CreateModel(out var x, out var y);

        Assert.Null(model.WithWarmStart(WarmStartFor(x, y)).VariableAttributes);
    }

    [Fact]
    public void WithWarmStartKeepsVariables()
    {
        var model = CreateModel(out var x, out var y);

        Assert.Equal(model.Variables, model.WithWarmStart(WarmStartFor(x, y)).Variables);
    }

    [Fact]
    public void WithWarmStartKeepsConstraints()
    {
        var model = CreateModel(out var x, out var y);

        Assert.Equal(model.Constraints, model.WithWarmStart(WarmStartFor(x, y)).Constraints);
    }

    [Fact]
    public void WithWarmStartKeepsObjectiveFunction()
    {
        var model = CreateModel(out var x, out var y);

        Assert.Equal(model.ObjectiveFunction, model.WithWarmStart(WarmStartFor(x, y)).ObjectiveFunction);
    }

    [Fact]
    public void WithWarmStartDoesNotModifyOriginalModel()
    {
        var model = CreateModel(out var x, out var y);

        _ = model.WithWarmStart(WarmStartFor(x, y));

        Assert.Null(model.WarmStart);
    }

    [Fact]
    public void WithWarmStartAcceptsPartialWarmStart()
    {
        var model = CreateModel(out var x, out _);
        var partialWarmStart = new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new IntegerScalar(1));

        Assert.Equal(partialWarmStart, model.WithWarmStart(partialWarmStart).WarmStart);
    }

    [Fact]
    public void WithWarmStartThrowsWhenVariableIsNotPartOfModel()
    {
        var model = CreateModel(out _, out _);
        var warmStart = new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>()
            .Add(IntegerVariable<IRealScalar>.Create(new RealInterval(0, 1), "foreign"), new IntegerScalar(1));

        Assert.Throws<VariableNotInModelException<IRealScalar>>(() => model.WithWarmStart(warmStart));
    }

    [Fact]
    public void WithVariableAttributesSetsAttributes()
    {
        var model = CreateModel(out _, out var y);
        var attributes = AttributesFor(y);

        Assert.Equal(attributes, model.WithVariableAttributes(attributes).VariableAttributes);
    }

    [Fact]
    public void WithVariableAttributesKeepsWarmStart()
    {
        var model = CreateModel(out var x, out var y);
        var warmStart = WarmStartFor(x, y);

        Assert.Equal(warmStart, model.WithWarmStart(warmStart).WithVariableAttributes(AttributesFor(y)).WarmStart);
    }

    [Fact]
    public void WithWarmStartKeepsVariableAttributes()
    {
        var model = CreateModel(out var x, out var y);
        var attributes = AttributesFor(y);

        Assert.Equal(attributes,
            model.WithVariableAttributes(attributes).WithWarmStart(WarmStartFor(x, y)).VariableAttributes);
    }

    [Fact]
    public void WithVariableAttributesThrowsWhenVariableIsNotPartOfModel()
    {
        var model = CreateModel(out _, out _);
        var attributes = new VariableAttributes<IIntegerVariable<IRealScalar>>()
            .Add(new BinaryVariable("foreign"), VariableAttributeType.HintValue, 1);

        Assert.Throws<VariableNotInModelException<IRealScalar>>(() => model.WithVariableAttributes(attributes));
    }

    [Fact]
    public void ModelsWithSameWarmStartAreEqual()
    {
        var first = CreateModel(out var x1, out var y1).WithWarmStart(WarmStartFor(x1, y1));
        var second = CreateModel(out var x2, out var y2).WithWarmStart(WarmStartFor(x2, y2));

        Assert.Equal(first, second);
    }

    [Fact]
    public void ModelsWithSameWarmStartHaveSameHashCode()
    {
        var first = CreateModel(out var x1, out var y1).WithWarmStart(WarmStartFor(x1, y1));
        var second = CreateModel(out var x2, out var y2).WithWarmStart(WarmStartFor(x2, y2));

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ModelWithWarmStartIsNotEqualToModelWithoutWarmStart()
    {
        var model = CreateModel(out var x, out var y);

        Assert.NotEqual(model, model.WithWarmStart(WarmStartFor(x, y)));
    }

    [Fact]
    public void ModelsWithDifferentWarmStartsAreNotEqual()
    {
        var model = CreateModel(out var x, out var y);
        var otherWarmStart = new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new IntegerScalar(2));

        Assert.NotEqual(model.WithWarmStart(WarmStartFor(x, y)), model.WithWarmStart(otherWarmStart));
    }

    [Fact]
    public void ModelWithVariableAttributesIsNotEqualToModelWithoutAttributes()
    {
        var model = CreateModel(out _, out var y);

        Assert.NotEqual(model, model.WithVariableAttributes(AttributesFor(y)));
    }

    [Fact]
    public void ToWarmStartContainsAllSolutionValues()
    {
        _ = CreateModel(out var x, out var y);
        var solutionValues = SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            (x, new RealScalar(2)), (y, new RealScalar(1)));

        Assert.Equal(
            new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new RealScalar(2)).Add(y, new RealScalar(1)),
            solutionValues.ToWarmStart());
    }

    private static WarmStart<IIntegerVariable<IRealScalar>, IRealScalar> WarmStartFor(
        IIntegerVariable<IRealScalar> x, IIntegerVariable<IRealScalar> y) =>
        new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new IntegerScalar(1)).Add(y, BinaryScalar.Zero);

    private static VariableAttributes<IIntegerVariable<IRealScalar>> AttributesFor(IIntegerVariable<IRealScalar> y) =>
        new VariableAttributes<IIntegerVariable<IRealScalar>>().Add(y, VariableAttributeType.BranchPriority, 3);

    private static ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar> CreateModel(
        out IIntegerVariable<IRealScalar> x, out IIntegerVariable<IRealScalar> y)
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>();
        x = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 3), "x");
        y = model.NewBinaryVariable<BinaryVariable>("y");

        model.AddConstraint(model.CreateConstraintBuilder()
            .AddTermToSum(new IntegerScalar(1), x)
            .AddTermToSum(new IntegerScalar(1), y)
            .Build(new RealInterval(0, 3)));

        return model.SetObjective(model.CreateObjectiveFunctionBuilder()
            .AddTermToSum(new IntegerScalar(2), x)
            .AddTermToSum(new IntegerScalar(1), y)
            .Build(true));
    }
}
