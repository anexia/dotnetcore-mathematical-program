// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverParameterTest.cs" company = "ANEXIA Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Tests.SolverConfiguration;

public sealed class SolverParameterTest
{
    [Fact]
    public void TimeLimitConstructorSetsDefaultSolverParameters()
    {
        var timeLimit = new TimeLimitInMilliseconds(2_500);
        var parameters = new SolverParameter(timeLimit);

        Assert.Equal(EnableSolverOutput.False, parameters.EnableSolverOutput);
        Assert.Null(parameters.RelativeGap);
        Assert.Equal(timeLimit, parameters.TimeLimitInMilliseconds);
        Assert.Equal(2u, parameters.TimeLimitInMilliseconds?.AsSeconds);
        Assert.Null(parameters.NumberOfThreads);
    }

    [Fact]
    public void RelativeGapFromEMinusReturnsExpectedPowerOfTen()
    {
        var gap = RelativeGap.FromEMinus(4);

        Assert.Equal(0.0001, gap.Value);
    }

    [Fact]
    public void ToSolverSpecificParametersForIlpIncludesMappedAndAdditionalParameters()
    {
        var parameters = new SolverParameter(
            EnableSolverOutput.True,
            RelativeGap: new RelativeGap(0.05),
            NumberOfThreads: new NumberOfThreads(8),
            AdditionalSolverSpecificParameters: [("custom", "value")]);

        var result = parameters.ToSolverSpecificParameters(IlpSolverType.HiGhs);

        Assert.Equal("threads=8,mip_rel_gap=0.05,custom=value", result);
    }

    [Fact]
    public void ToSolverSpecificParametersForLpUsesSolverSpecificSeparators()
    {
        var parameters = new SolverParameter(
            EnableSolverOutput.False,
            RelativeGap: new RelativeGap(0.05),
            NumberOfThreads: new NumberOfThreads(3),
            AdditionalSolverSpecificParameters: [("custom", "value")]);

        Assert.Equal("num_omp_threads:3,custom:value", parameters.ToSolverSpecificParameters(LpSolverType.Glop));
        Assert.Equal("parallel/maxnthreads=3,custom=value", parameters.ToSolverSpecificParameters(LpSolverType.Scip));
    }
}
