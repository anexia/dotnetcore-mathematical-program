// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverParameter.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// Represents the parameters that can be set for a solver.
/// </summary>
/// <param name="EnableSolverOutput">Whether to enable the solver's underlying log output.</param>
/// <param name="TimeLimitInMilliseconds">Time limit of the solving process.</param>
/// <param name="NumberOfThreads">The number of threads that should be used by the solver.</param>
/// <param name="RelativeGap">The relative gap when the solver should terminate.</param>
/// <param name="ExportModelFilePath">The file path and name of a file where the model should be written to.
/// Use .txt extension for a human-readable format, otherwise it is saved as binary file.</param>
public record SolverParameter(
    EnableSolverOutput EnableSolverOutput,
    RelativeGap? RelativeGap = null,
    TimeLimitInMilliseconds? TimeLimitInMilliseconds = null,
    NumberOfThreads? NumberOfThreads = null,
    string? ExportModelFilePath = null)
{
    private const string RelativeGapKey = "RELATIVE_GAP";
    private const string NumberOfThreadsKey = "NUMBER_OF_THREADS_KEY";

    private static readonly Dictionary<(IlpSolverType, string), string> IlpParameterKeyMapping =
        new()
        {
            { (IlpSolverType.CbcIntegerProgramming, RelativeGapKey), "gapratio" },
            { (IlpSolverType.CbcIntegerProgramming, NumberOfThreadsKey), "threads" },
            { (IlpSolverType.GurobiIntegerProgramming, RelativeGapKey), "MIPGap" },
            { (IlpSolverType.GurobiIntegerProgramming, NumberOfThreadsKey), "Threads" },
            { (IlpSolverType.Scip, RelativeGapKey), "limits/gap" },
            { (IlpSolverType.Scip, NumberOfThreadsKey), "parallel/maxnthreads" }
        };

    private static readonly Dictionary<(LpSolverType, string), string> LpParameterKeyMapping =
        new()
        {
            { (LpSolverType.Glop, RelativeGapKey), "solution_feasibility_tolerance" },
            { (LpSolverType.Glop, NumberOfThreadsKey), "num_omp_threads" },
            { (LpSolverType.GurobiIntegerProgramming, RelativeGapKey), "MIPGap" },
            { (LpSolverType.GurobiIntegerProgramming, NumberOfThreadsKey), "Threads" },
            { (LpSolverType.Scip, RelativeGapKey), "limits/gap" },
            { (LpSolverType.Scip, NumberOfThreadsKey), "parallel/maxnthreads" }
        };

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
        : this(EnableSolverOutput.False, null, timeLimitInMilliseconds)
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
    /// <item><term>RelativeGap</term><description>Solver's default value</description></item>
    /// </list>
    public SolverParameter()
        : this(EnableSolverOutput.False)
    {
    }

    internal string ToSolverSpecificParameters(IlpSolverType solverType)
    {
        var parameters = new List<string>();
        if (NumberOfThreads is not null)
            parameters.Add(
                $"{IlpParameterKeyMapping[(solverType, NumberOfThreadsKey)]}={NumberOfThreads.Value}");
        if (RelativeGap is not null)
            parameters.Add(
                $"{IlpParameterKeyMapping[(solverType, RelativeGapKey)]}={RelativeGap.Value}");

        return string.Join(',', parameters);
    }

    internal string ToSolverSpecificParameters(LpSolverType solverType)
    {
        var parameters = new List<string>();
        if (NumberOfThreads is not null)
            parameters.Add(
                $"{LpParameterKeyMapping[(solverType, NumberOfThreadsKey)]}={NumberOfThreads.Value}");

        return string.Join(',', parameters);
    }

    /// <inderitdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() =>
        $"{nameof(EnableSolverOutput)}: {EnableSolverOutput}, {nameof(RelativeGap)}: {RelativeGap}, {nameof(TimeLimitInMilliseconds)}: {TimeLimitInMilliseconds}, {nameof(NumberOfThreads)}: {NumberOfThreads}, {nameof(ExportModelFilePath)}: {ExportModelFilePath}";
}