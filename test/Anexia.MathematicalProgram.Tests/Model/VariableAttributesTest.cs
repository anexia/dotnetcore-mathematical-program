// ------------------------------------------------------------------------------------------
//  <copyright file = "VariableAttributesTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Hint;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class VariableAttributesTest
{
    private static readonly IIntegerVariable<IRealScalar> X =
        IntegerVariable<IRealScalar>.Create(new RealInterval(0, 3), "x");

    private static readonly IIntegerVariable<IRealScalar> Y = new BinaryVariable("y");

    [Fact]
    public void NewAttributesAreEmpty() => Assert.True(new VariableAttributes<IIntegerVariable<IRealScalar>>().Empty);

    [Fact]
    public void NewAttributesHaveNoEntries() => Assert.Empty(new VariableAttributes<IIntegerVariable<IRealScalar>>());

    [Fact]
    public void AddReturnsAttributesWithOneEntryPerVariableAndType() => Assert.Equal(3, ThreeAttributes().Count);

    [Fact]
    public void AddedAttributesAreNotEmpty() => Assert.False(ThreeAttributes().Empty);

    [Fact]
    public void AddedHintValueCanBeRetrieved()
    {
        ThreeAttributes().TryGetValue(X, VariableAttributeType.HintValue, out var value);

        Assert.Equal(2, value);
    }

    [Fact]
    public void AddedHintPriorityCanBeRetrieved()
    {
        ThreeAttributes().TryGetValue(X, VariableAttributeType.HintPriority, out var value);

        Assert.Equal(5, value);
    }

    [Fact]
    public void AddedBranchPriorityCanBeRetrieved()
    {
        ThreeAttributes().TryGetValue(Y, VariableAttributeType.BranchPriority, out var value);

        Assert.Equal(10, value);
    }

    [Fact]
    public void TryGetValueReturnsTrueForKnownAttribute() =>
        Assert.True(ThreeAttributes().TryGetValue(X, VariableAttributeType.HintValue, out _));

    [Fact]
    public void TryGetValueReturnsFalseForMissingAttributeType() =>
        Assert.False(ThreeAttributes().TryGetValue(X, VariableAttributeType.BranchPriority, out _));

    [Fact]
    public void TryGetValueReturnsFalseForMissingVariable() =>
        Assert.False(ThreeAttributes().TryGetValue(Y, VariableAttributeType.HintValue, out _));

    [Fact]
    public void AddDoesNotModifyOriginalAttributes()
    {
        var original = new VariableAttributes<IIntegerVariable<IRealScalar>>();

        _ = original.Add(X, VariableAttributeType.HintValue, 2);

        Assert.True(original.Empty);
    }

    [Fact]
    public void AddingSameVariableAndTypeTwiceKeepsSingleEntry() => Assert.Single(SameAttributeAddedTwice());

    [Fact]
    public void AddingSameVariableAndTypeTwiceReplacesValue()
    {
        SameAttributeAddedTwice().TryGetValue(X, VariableAttributeType.HintValue, out var value);

        Assert.Equal(3, value);
    }

    [Fact]
    public void EnumerationYieldsAllAttributes() =>
        Assert.Equivalent(
            new[]
            {
                new VariableAttribute<IIntegerVariable<IRealScalar>>(X, VariableAttributeType.HintValue, 2),
                new VariableAttribute<IIntegerVariable<IRealScalar>>(X, VariableAttributeType.HintPriority, 5),
                new VariableAttribute<IIntegerVariable<IRealScalar>>(Y, VariableAttributeType.BranchPriority, 10)
            }, ThreeAttributes().ToArray());

    [Fact]
    public void AttributesWithSameEntriesInDifferentOrderAreEqual() =>
        Assert.Equal(ThreeAttributes(), ThreeAttributesInReverseOrder());

    [Fact]
    public void AttributesWithSameEntriesInDifferentOrderHaveSameHashCode() =>
        Assert.Equal(ThreeAttributes().GetHashCode(), ThreeAttributesInReverseOrder().GetHashCode());

    [Fact]
    public void AttributesWithDifferentValuesAreNotEqual() =>
        Assert.NotEqual(
            new VariableAttributes<IIntegerVariable<IRealScalar>>().Add(X, VariableAttributeType.HintValue, 2),
            new VariableAttributes<IIntegerVariable<IRealScalar>>().Add(X, VariableAttributeType.HintValue, 3));

    [Fact]
    public void AttributesWithDifferentTypesAreNotEqual() =>
        Assert.NotEqual(
            new VariableAttributes<IIntegerVariable<IRealScalar>>().Add(X, VariableAttributeType.HintValue, 2),
            new VariableAttributes<IIntegerVariable<IRealScalar>>().Add(X, VariableAttributeType.HintPriority, 2));

    [Fact]
    public void AttributesAreNotEqualToNull() => Assert.False(ThreeAttributes().Equals(null));

    [Fact]
    public void AttributesAreNotEqualToOtherType() => Assert.False(ThreeAttributes().Equals(new object()));

    private static VariableAttributes<IIntegerVariable<IRealScalar>> ThreeAttributes() =>
        new VariableAttributes<IIntegerVariable<IRealScalar>>()
            .Add(X, VariableAttributeType.HintValue, 2)
            .Add(X, VariableAttributeType.HintPriority, 5)
            .Add(Y, VariableAttributeType.BranchPriority, 10);

    private static VariableAttributes<IIntegerVariable<IRealScalar>> ThreeAttributesInReverseOrder() =>
        new VariableAttributes<IIntegerVariable<IRealScalar>>()
            .Add(Y, VariableAttributeType.BranchPriority, 10)
            .Add(X, VariableAttributeType.HintPriority, 5)
            .Add(X, VariableAttributeType.HintValue, 2);

    private static VariableAttributes<IIntegerVariable<IRealScalar>> SameAttributeAddedTwice() =>
        new VariableAttributes<IIntegerVariable<IRealScalar>>()
            .Add(X, VariableAttributeType.HintValue, 2)
            .Add(X, VariableAttributeType.HintValue, 3);
}
