// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverParameter.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// Represents the parameters that can be set for a solver.
/// </summary>
/// <param name="enableSolverOutput">Whether to enable the solver's underlying log output.</param>
/// <param name="timeLimitInMilliseconds">Time limit of the solving process.</param>
/// <param name="numberOfThreads">The number of threads that should be used by the solver.</param>
/// <param name="relativeGap">The relative gap when the solver should terminate.</param>
public sealed class SolverParameter(
    EnableSolverOutput enableSolverOutput,
    RelativeGap relativeGap,
    TimeLimitInMilliseconds? timeLimitInMilliseconds = null,
    NumberOfThreads? numberOfThreads = null) : MemberwiseEquatable<SolverParameter>
{
    /// <summary>
    /// Create default solver parameters with given time limit.
    /// </summary>
    /// <param name="timeLimitInMilliseconds">The time limit in milliseconds.</param>
    /// <list type="table">
    ///     <listheader><term>Parameter</term><description>Description</description></listheader>
    /// <item><term>NumberOfThreads</term><description>Default value: null</description></item>
    /// <item><term>EnableSolverOutput</term><description>Default value: false</description></item>
    /// <item><term>RelativeGap</term><description>Default value: E-7</description></item>
    /// </list>
    public SolverParameter(TimeLimitInMilliseconds timeLimitInMilliseconds)
        : this(EnableSolverOutput.False, RelativeGap.EMinus7, timeLimitInMilliseconds, new NumberOfThreads(0))
    {
    }

    /// <summary>
    /// Create solver parameters without a time limit.
    /// </summary>
    /// <list type="table">
    ///     <listheader><term>Parameter</term><description>Description</description></listheader>
    /// <item><term>TimeLimitInMilliseconds</term><description>Default value: null</description></item>
    /// <item><term>NumberOfThreads</term><description>Default value: null</description></item>
    /// <item><term>EnableSolverOutput</term><description>Default value: false</description></item>
    /// <item><term>RelativeGap</term><description>Default value: E-7</description></item>
    /// </list>
    public SolverParameter(EnableSolverOutput enableSolverOutput,
        NumberOfThreads numberOfThreads,
        RelativeGap relativeGap)
        : this(enableSolverOutput, relativeGap, null, numberOfThreads)
    {
    }

    /// <summary>
    /// Create default solver parameters
    /// </summary>
    /// <list type="table">
    ///     <listheader><term>Parameter</term><description>Description</description></listheader>
    /// <item><term>TimeLimitInMilliseconds</term><description>Default value: null</description></item>
    /// <item><term>NumberOfThreads</term><description>Default value: null</description></item>
    /// <item><term>EnableSolverOutput</term><description>Default value: false</description></item>
    /// <item><term>RelativeGap</term><description>Default value: E-7</description></item>
    /// </list>
    public SolverParameter()
        : this(EnableSolverOutput.False, new NumberOfThreads(0), RelativeGap.EMinus7)
    {
    }

    public RelativeGap RelativeGap { get; } = relativeGap;
    public TimeLimitInMilliseconds? TimeLimitInMilliseconds { get; } = timeLimitInMilliseconds;
    public EnableSolverOutput EnableSolverOutput { get; } = enableSolverOutput;
    public NumberOfThreads? NumberOfThreads { get; } = numberOfThreads;

    /// <inderitdoc />
    public override string ToString() =>
        $"{nameof(RelativeGap)}: {RelativeGap}, {nameof(TimeLimitInMilliseconds)}: {TimeLimitInMilliseconds}, {nameof(EnableSolverOutput)}: {EnableSolverOutput}, {nameof(NumberOfThreads)}: {NumberOfThreads}";
}