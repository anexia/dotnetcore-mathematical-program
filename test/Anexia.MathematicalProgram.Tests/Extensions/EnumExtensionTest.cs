// ------------------------------------------------------------------------------------------
//  <copyright file = "EnumExtensionTest.cs" company = "ANEXIA Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Tests.Extensions;

public sealed class EnumExtensionTest
{
    [Fact]
    public void ToEnumStringReturnsEnumMemberValue()
    {
        Assert.Equal("GLOP", LpSolverType.Glop.ToEnumString());
        Assert.Equal("HIGHS_MIXED_INTEGER_PROGRAMMING", IlpSolverType.HiGhs.ToEnumString());
    }

    [Fact]
    public void ToEnumStringReturnsEmptyStringWhenEnumMemberAttributeIsMissing()
    {
        Assert.Equal(string.Empty, SolverResultStatus.Optimal.ToEnumString());
    }

    [Fact]
    public void ToEnumStringReturnsEmptyStringForInvalidEnumValue()
    {
        Assert.Equal(string.Empty, ((LpSolverType)int.MaxValue).ToEnumString());
    }
}
