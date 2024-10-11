// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverParameter.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.SolverConfiguration;

public sealed class SolverParameter(
    TimeLimitInMilliseconds timeLimitInMilliseconds,
    EnableSolverOutput enableSolverOutput,
    NumberOfThreads numberOfThreads,
    RelativeGap relativeGap) : MemberwiseEquatable<SolverParameter>
{
    /// <summary>
    /// Create default solver parameters with given time limit
    /// </summary>
    /// <param name="timeLimitInMilliseconds">The time limit in milliseconds</param>
    /// <list type="table">
    ///     <listheader><term>Parameter</term><description>Description</description></listheader>
    /// <item><term>NumberOfThreads</term><description>Default value: false</description></item>
    /// <item><term>EnableSolverOutput</term><description>Default value: 0</description></item>
    /// <item><term>RelativeGap</term><description>Default value: E-7</description></item>
    /// </list>
    public SolverParameter(TimeLimitInMilliseconds timeLimitInMilliseconds)
        : this(timeLimitInMilliseconds, EnableSolverOutput.False, new NumberOfThreads(0), RelativeGap.EMinus7)
    { }

    /// <summary>
    /// Create default solver parameters
    /// </summary>
    /// <list type="table">
    ///     <listheader><term>Parameter</term><description>Description</description></listheader>
    /// <item><term>TimeLimitInMilliseconds</term><description>Default value: Unbounded</description></item>
    /// <item><term>NumberOfThreads</term><description>Default value: false</description></item>
    /// <item><term>EnableSolverOutput</term><description>Default value: 0</description></item>
    /// <item><term>RelativeGap</term><description>Default value: E-7</description></item>
    /// </list>
    public SolverParameter()
        : this(TimeLimitInMilliseconds.Unbounded, EnableSolverOutput.False, new NumberOfThreads(0), RelativeGap.EMinus7)
    { }

    public RelativeGap RelativeGap { get; } = relativeGap;
    public TimeLimitInMilliseconds TimeLimitInMilliseconds { get; } = timeLimitInMilliseconds;
    public EnableSolverOutput EnableSolverOutput { get; } = enableSolverOutput;
    public NumberOfThreads NumberOfThreads { get; } = numberOfThreads;

    public override string ToString() =>
        $"{nameof(RelativeGap)}: {RelativeGap}, {nameof(TimeLimitInMilliseconds)}: {TimeLimitInMilliseconds}, {nameof(EnableSolverOutput)}: {EnableSolverOutput}, {nameof(NumberOfThreads)}: {NumberOfThreads}";
}