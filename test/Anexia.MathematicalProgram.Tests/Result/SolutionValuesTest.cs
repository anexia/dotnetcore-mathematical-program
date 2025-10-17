// ------------------------------------------------------------------------------------------
//  <copyright file = "SolutionValuesTest.cs" company = "ANEXIA Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;

namespace Anexia.MathematicalProgram.Tests.Result;

public sealed class SolutionValuesTest
{
    [Fact]
    public void EmptySolutionValuesReturnsDefaultForMissingVariables()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        var variable = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 1), "v1");
        var values = new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            ReadOnlyDictionary<IIntegerVariable<IRealScalar>, RealScalar>.Empty);

        Assert.True(values.Empty);
        Assert.Null(values.GetSolutionValueOrDefault(variable));
        Assert.False(values.TryGetSolutionValue(variable, out var value));
        Assert.Null(value);
        Assert.Empty(values);
    }

    [Fact]
    public void SolutionValuesReturnsAndEnumeratesStoredValues()
    {
        var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        var firstVariable = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(0, 10), "v1");
        var secondVariable = model.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(-10, 10), "v2");
        var values = new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            new ReadOnlyDictionary<IIntegerVariable<IRealScalar>, RealScalar>(
                new Dictionary<IIntegerVariable<IRealScalar>, RealScalar>
                {
                    [firstVariable] = new(2.5),
                    [secondVariable] = new(-3)
                }));

        Assert.False(values.Empty);
        Assert.Equal(new RealScalar(2.5), values.GetSolutionValueOrDefault(firstVariable));
        Assert.True(values.TryGetSolutionValue(secondVariable, out var secondValue));
        Assert.Equal(new RealScalar(-3), secondValue);
        Assert.Collection(
            values.OrderBy(pair => pair.Key.Name),
            first =>
            {
                Assert.Equal("v1", first.Key.Name);
                Assert.Equal(new RealScalar(2.5), first.Value);
            },
            second =>
            {
                Assert.Equal("v2", second.Key.Name);
                Assert.Equal(new RealScalar(-3), second.Value);
            });
    }
}
