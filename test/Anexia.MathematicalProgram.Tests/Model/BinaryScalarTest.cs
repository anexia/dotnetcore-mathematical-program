// ------------------------------------------------------------------------------------------
//  <copyright file = "BinaryScalarTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class BinaryScalarTest
{
    public static IEnumerable<object[]> BinaryPairs()
    {
        yield return [BinaryScalar.Zero, BinaryScalar.Zero, BinaryScalar.Zero];
        yield return [BinaryScalar.Zero, BinaryScalar.One, BinaryScalar.One];
        yield return [BinaryScalar.One, BinaryScalar.Zero, BinaryScalar.One];
        yield return [BinaryScalar.One, BinaryScalar.One, BinaryScalar.Zero];
    }

    public static IEnumerable<object[]> BinaryRealPairs()
    {
        yield return [BinaryScalar.Zero, new RealScalar(0), new RealScalar(0)];
        yield return [BinaryScalar.Zero, new RealScalar(1), new RealScalar(1)];
        yield return [BinaryScalar.One, new RealScalar(0), new RealScalar(1)];
        yield return [BinaryScalar.One, new RealScalar(1), new RealScalar(0)];
    }

    [Theory]
    [MemberData(nameof(BinaryPairs))]
    public void AddWithBinaryScalarReturnsExpected(BinaryScalar left, BinaryScalar right, BinaryScalar expected)
    {
        var result = left.Add(right);

        Assert.Equal(expected.IsOne, result.IsOne);
        Assert.Equal(expected.Value, result.Value);
    }

    [Theory]
    [MemberData(nameof(BinaryPairs))]
    public void SubtractWithBinaryScalarReturnsExpected(BinaryScalar left, BinaryScalar right, BinaryScalar expected)
    {
        var result = left.Subtract(right);

        Assert.Equal(expected.IsOne, result.IsOne);
        Assert.Equal(expected.Value, result.Value);
    }

    [Theory]
    [MemberData(nameof(BinaryPairs))]
    public void AddWithIBinaryScalarReturnsExpected(BinaryScalar left, BinaryScalar right, BinaryScalar expected)
    {
        IBinaryScalar r = right;
        var result = left.Add(r);

        Assert.Equal(expected.IsOne, result.IsOne);
        Assert.Equal(expected.Value, result.Value);
    }

    [Theory]
    [MemberData(nameof(BinaryRealPairs))]
    public void AddWithIRealScalarReturnsExpected(BinaryScalar left, IRealScalar right, IRealScalar expected)
    {
        var result = left.Add(right);

        Assert.Equal(expected.Value, result.Value);
    }

    [Theory]
    [MemberData(nameof(BinaryPairs))]
    public void SubtractWithIBinaryScalarReturnsExpected(BinaryScalar left, BinaryScalar right, BinaryScalar expected)
    {
        IBinaryScalar r = right;
        var result = left.Subtract(r);

        Assert.Equal(expected.IsOne, result.IsOne);
        Assert.Equal(expected.Value, result.Value);
    }

    [Theory]
    [MemberData(nameof(BinaryRealPairs))]
    public void SubtractWithIRealScalarReturnsExpected(BinaryScalar left, IRealScalar right, IRealScalar expected)
    {
        var result = left.Subtract(right);

        Assert.Equal(expected.Value, result.Value);
    }

    [Theory]
    [MemberData(nameof(BinaryPairs))]
    public void OperatorPlusReturnsExpected(BinaryScalar left, BinaryScalar right, BinaryScalar expected)
    {
        var result = left + right;

        Assert.Equal(expected.IsOne, result.IsOne);
        Assert.Equal(expected.Value, result.Value);
    }

    [Theory]
    [MemberData(nameof(BinaryPairs))]
    public void OperatorMinusReturnsExpected(BinaryScalar left, BinaryScalar right, BinaryScalar expected)
    {
        var result = left - right;

        Assert.Equal(expected.IsOne, result.IsOne);
        Assert.Equal(expected.Value, result.Value);
    }
}