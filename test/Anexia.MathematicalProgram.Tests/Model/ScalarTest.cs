// ------------------------------------------------------------------------------------------
//  <copyright file = "ScalarTest.cs" company = "ANEXIA Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class ScalarTest
{
    [Fact]
    public void RealScalarSupportsConcreteAndInterfaceArithmetic()
    {
        var left = new RealScalar(7.5);
        var right = new RealScalar(2);
        IRealScalar interfaceRight = right;

        Assert.Equal(new RealScalar(9.5), left.Add(right));
        Assert.Equal(new RealScalar(5.5), left.Subtract(right));
        Assert.Equal(9.5, left.Add(interfaceRight).Value);
        Assert.Equal(5.5, left.Subtract(interfaceRight).Value);
        Assert.Equal(new RealScalar(9.5), left + right);
        Assert.Equal(new RealScalar(5.5), left - right);
    }

    [Fact]
    public void IntegerScalarSupportsConcreteAndInterfaceArithmetic()
    {
        var left = new IntegerScalar(7);
        var right = new IntegerScalar(2);
        IIntegerScalar integerRight = right;
        IRealScalar realRight = new RealScalar(2.5);

        Assert.Equal(new IntegerScalar(9), left.Add(right));
        Assert.Equal(new IntegerScalar(5), left.Subtract(right));
        Assert.Equal(9, left.Add(integerRight).Value);
        Assert.Equal(5, left.Subtract(integerRight).Value);
        Assert.Equal(9.5, left.Add(realRight).Value);
        Assert.Equal(4.5, left.Subtract(realRight).Value);
        Assert.Equal(new IntegerScalar(9), left + right);
        Assert.Equal(new IntegerScalar(5), left - right);
    }

    [Fact]
    public void BinaryScalarSupportsIntegerScalarArithmetic()
    {
        IIntegerScalar even = new IntegerScalar(4);
        IIntegerScalar odd = new IntegerScalar(3);

        Assert.Equal(1, BinaryScalar.One.Add(even).Value);
        Assert.Equal(0, BinaryScalar.One.Add(odd).Value);
        Assert.Equal(1, BinaryScalar.One.Subtract(even).Value);
        Assert.Equal(1, BinaryScalar.Zero.Subtract(odd).Value);
    }
}
