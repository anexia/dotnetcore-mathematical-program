// ------------------------------------------------------------------------------------------
//  <copyright file = "WarmStartTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Hint;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class WarmStartTest
{
    private static readonly IIntegerVariable<IRealScalar> X =
        IntegerVariable<IRealScalar>.Create(new RealInterval(0, 3), "x");

    private static readonly IIntegerVariable<IRealScalar> Y = new BinaryVariable("y");

    [Fact]
    public void NewWarmStartIsEmpty() => Assert.True(new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Empty);

    [Fact]
    public void NewWarmStartHasNoStartValues() =>
        Assert.Empty(new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>());

    [Fact]
    public void AddReturnsWarmStartWithOneStartValuePerVariable() => Assert.Equal(2, TwoStartValues().Count);

    [Fact]
    public void AddedStartValueIsNotEmpty() => Assert.False(TwoStartValues().Empty);

    [Fact]
    public void AddedIntegerStartValueCanBeRetrieved()
    {
        TwoStartValues().TryGetStartValue(X, out var value);

        Assert.Equal(new IntegerScalar(2), value);
    }

    [Fact]
    public void AddedBinaryStartValueCanBeRetrieved()
    {
        TwoStartValues().TryGetStartValue(Y, out var value);

        Assert.Equal(BinaryScalar.One, value);
    }

    [Fact]
    public void TryGetStartValueReturnsTrueForKnownVariable() =>
        Assert.True(TwoStartValues().TryGetStartValue(X, out _));

    [Fact]
    public void TryGetStartValueReturnsFalseForUnknownVariable() =>
        Assert.False(new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(X, new IntegerScalar(2))
            .TryGetStartValue(Y, out _));

    [Fact]
    public void AddDoesNotModifyOriginalWarmStart()
    {
        var original = new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>();

        _ = original.Add(X, new IntegerScalar(2));

        Assert.True(original.Empty);
    }

    [Fact]
    public void AddingSameVariableTwiceKeepsSingleStartValue() => Assert.Single(SameVariableAddedTwice());

    [Fact]
    public void AddingSameVariableTwiceReplacesStartValue()
    {
        SameVariableAddedTwice().TryGetStartValue(X, out var value);

        Assert.Equal(new IntegerScalar(3), value);
    }

    [Fact]
    public void EnumerationYieldsAllStartValues() =>
        Assert.Equivalent(
            new[]
            {
                new StartValue<IIntegerVariable<IRealScalar>, IRealScalar>(X, new IntegerScalar(2)),
                new StartValue<IIntegerVariable<IRealScalar>, IRealScalar>(Y, BinaryScalar.One)
            }, TwoStartValues().ToArray());

    [Fact]
    public void WarmStartFromPairsEqualsWarmStartFromAdd() => Assert.Equal(TwoStartValues(), TwoStartValuesFromPairs());

    [Fact]
    public void WarmStartFromPairsHasSameHashCodeAsWarmStartFromAdd() =>
        Assert.Equal(TwoStartValues().GetHashCode(), TwoStartValuesFromPairs().GetHashCode());

    [Fact]
    public void WarmStartsWithDifferentValuesAreNotEqual() =>
        Assert.NotEqual(
            new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(X, new IntegerScalar(2)),
            new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(X, new IntegerScalar(3)));

    [Fact]
    public void WarmStartsWithDifferentVariablesAreNotEqual() =>
        Assert.NotEqual(
            new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(X, new IntegerScalar(1)),
            new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>().Add(Y, new IntegerScalar(1)));

    [Fact]
    public void WarmStartIsNotEqualToNull() => Assert.False(TwoStartValues().Equals(null));

    [Fact]
    public void WarmStartIsNotEqualToOtherType() => Assert.False(TwoStartValues().Equals(new object()));

    private static WarmStart<IIntegerVariable<IRealScalar>, IRealScalar> TwoStartValues() =>
        new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>()
            .Add(X, new IntegerScalar(2))
            .Add(Y, BinaryScalar.One);

    private static WarmStart<IIntegerVariable<IRealScalar>, IRealScalar> TwoStartValuesFromPairs() =>
        new(
        [
            new KeyValuePair<IIntegerVariable<IRealScalar>, IRealScalar>(Y, BinaryScalar.One),
            new KeyValuePair<IIntegerVariable<IRealScalar>, IRealScalar>(X, new IntegerScalar(2))
        ]);

    private static WarmStart<IIntegerVariable<IRealScalar>, IRealScalar> SameVariableAddedTwice() =>
        new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>()
            .Add(X, new IntegerScalar(2))
            .Add(X, new IntegerScalar(3));
}
