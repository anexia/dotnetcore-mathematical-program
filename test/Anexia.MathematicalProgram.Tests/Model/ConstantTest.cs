// ------------------------------------------------------------------------------------------
//  <copyright file = "ConstantTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;

#endregion

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class ConstantTest
{
    [Theory]
    [InlineData(5, 5, 10)]
    [InlineData(17, -7, 10)]
    [InlineData(-7, -7, -14)]
    [InlineData(double.MaxValue, -5, double.MaxValue)]
    [InlineData(double.MaxValue, -double.MaxValue, 0)]
    [InlineData(double.MaxValue, 5, double.MaxValue)]
    [InlineData(double.MinValue, -7, double.MinValue)]
    public void ConstantAdditionReturnsCorrectResult(double constant, double toAdd, double result) =>
        Assert.Equal(new Constant(constant) + toAdd, new Constant(result));
}