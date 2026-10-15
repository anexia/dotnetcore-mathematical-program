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
    public void CompletedModelHasNoStartSolutionByDefault() => Assert.Null(CreateModel(out _, out _).StartSolution);

    [Fact]
    public void CompletedModelHasNoVariableAttributesByDefault() =>
        Assert.Null(CreateModel(out _, out _).VariableAttributes);

    [Fact]
    public void WithStartSolutionSetsStartSolution()
    {
        var model = CreateModel(out var x, out var y);
        var startSolution = StartSolutionFor(x, y);

        Assert.Equal(startSolution, model.WithHint(startSolution).StartSolution);
    }

    [Fact]
    public void WithStartSolutionLeavesVariableAttributesUnset()
    {
        var model = CreateModel(out var x, out var y);

        Assert.Null(model.WithHint(StartSolutionFor(x, y)).VariableAttributes);
    }

    [Fact]
    public void WithStartSolutionKeepsVariables()
    {
        var model = CreateModel(out var x, out var y);

        Assert.Equal(model.Variables, model.WithHint(StartSolutionFor(x, y)).Variables);
    }

    [Fact]
    public void WithStartSolutionKeepsConstraints()
    {
        var model = CreateModel(out var x, out var y);

        Assert.Equal(model.Constraints, model.WithHint(StartSolutionFor(x, y)).Constraints);
    }

    [Fact]
    public void WithStartSolutionKeepsObjectiveFunction()
    {
        var model = CreateModel(out var x, out var y);

        Assert.Equal(model.ObjectiveFunction, model.WithHint(StartSolutionFor(x, y)).ObjectiveFunction);
    }

    [Fact]
    public void WithStartSolutionDoesNotModifyOriginalModel()
    {
        var model = CreateModel(out var x, out var y);

        _ = model.WithHint(StartSolutionFor(x, y));

        Assert.Null(model.StartSolution);
    }

    [Fact]
    public void WithStartSolutionAcceptsPartialStartSolution()
    {
        var model = CreateModel(out var x, out _);
        var partialStartSolution = new StartSolution<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new IntegerScalar(1));

        Assert.Equal(partialStartSolution, model.WithHint(partialStartSolution).StartSolution);
    }

    [Fact]
    public void WithStartSolutionThrowsWhenVariableIsNotPartOfModel()
    {
        var model = CreateModel(out _, out _);
        var startSolution = new StartSolution<IIntegerVariable<IRealScalar>, IRealScalar>()
            .Add(IntegerVariable<IRealScalar>.Create(new RealInterval(0, 1), "foreign"), new IntegerScalar(1));

        Assert.Throws<VariableNotInModelException<IRealScalar>>(() => model.WithHint(startSolution));
    }

    [Fact]
    public void WithVariableAttributesSetsAttributes()
    {
        var model = CreateModel(out _, out var y);
        var attributes = AttributesFor(y);

        Assert.Equal(attributes, model.WithVariableAttributes(attributes).VariableAttributes);
    }

    [Fact]
    public void WithVariableAttributesKeepsStartSolution()
    {
        var model = CreateModel(out var x, out var y);
        var startSolution = StartSolutionFor(x, y);

        Assert.Equal(startSolution, model.WithHint(startSolution).WithVariableAttributes(AttributesFor(y)).StartSolution);
    }

    [Fact]
    public void WithStartSolutionKeepsVariableAttributes()
    {
        var model = CreateModel(out var x, out var y);
        var attributes = AttributesFor(y);

        Assert.Equal(attributes,
            model.WithVariableAttributes(attributes).WithHint(StartSolutionFor(x, y)).VariableAttributes);
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
    public void ModelsWithSameStartSolutionAreEqual()
    {
        var first = CreateModel(out var x1, out var y1).WithHint(StartSolutionFor(x1, y1));
        var second = CreateModel(out var x2, out var y2).WithHint(StartSolutionFor(x2, y2));

        Assert.Equal(first, second);
    }

    [Fact]
    public void ModelsWithSameStartSolutionHaveSameHashCode()
    {
        var first = CreateModel(out var x1, out var y1).WithHint(StartSolutionFor(x1, y1));
        var second = CreateModel(out var x2, out var y2).WithHint(StartSolutionFor(x2, y2));

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ModelWithStartSolutionIsNotEqualToModelWithoutStartSolution()
    {
        var model = CreateModel(out var x, out var y);

        Assert.NotEqual(model, model.WithHint(StartSolutionFor(x, y)));
    }

    [Fact]
    public void ModelsWithDifferentStartSolutionsAreNotEqual()
    {
        var model = CreateModel(out var x, out var y);
        var otherStartSolution = new StartSolution<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new IntegerScalar(2));

        Assert.NotEqual(model.WithHint(StartSolutionFor(x, y)), model.WithHint(otherStartSolution));
    }

    [Fact]
    public void ModelWithVariableAttributesIsNotEqualToModelWithoutAttributes()
    {
        var model = CreateModel(out _, out var y);

        Assert.NotEqual(model, model.WithVariableAttributes(AttributesFor(y)));
    }

    [Fact]
    public void ToStartSolutionContainsAllSolutionValues()
    {
        _ = CreateModel(out var x, out var y);
        var solutionValues = SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            (x, new RealScalar(2)), (y, new RealScalar(1)));

        Assert.Equal(
            new StartSolution<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new RealScalar(2)).Add(y, new RealScalar(1)),
            solutionValues.ToStartSolution());
    }

    private static StartSolution<IIntegerVariable<IRealScalar>, IRealScalar> StartSolutionFor(
        IIntegerVariable<IRealScalar> x, IIntegerVariable<IRealScalar> y) =>
        new StartSolution<IIntegerVariable<IRealScalar>, IRealScalar>().Add(x, new IntegerScalar(1)).Add(y, BinaryScalar.Zero);

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
