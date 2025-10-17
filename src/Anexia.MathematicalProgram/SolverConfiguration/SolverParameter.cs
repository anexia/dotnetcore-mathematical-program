// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverParameter.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// Represents the parameters that can be set for a solver.
/// </summary>
/// <param name="EnableSolverOutput">Whether to enable the solver's underlying log output.</param>
/// <param name="TimeLimitInMilliseconds">Time limit of the solving process.</param>
/// <param name="NumberOfThreads">The number of threads that should be used by the solver.</param>
/// <param name="RelativeGap">The relative gap when the solver should terminate.</param>
/// <param name="AdditionalSolverSpecificParameters">Additional solver specific parameters (key-value string pairs) to pass to the solver. The correct format for the desired solver
/// must be used. Check corresponding solver documentations to be sure. E.g., Gurobi parameters can be found here: https://docs.gurobi.com/projects/optimizer/en/current/reference/parameters.html#secparameterreference</param>
/// <param name="ExportModelFilePath">The file path and name of a file where the model should be written to.</param>
public record SolverParameter(
    EnableSolverOutput EnableSolverOutput,
    RelativeGap? RelativeGap = null,
    TimeLimitInMilliseconds? TimeLimitInMilliseconds = null,
    NumberOfThreads? NumberOfThreads = null,
    IReadOnlyCollection<(string Key, string Value)>? AdditionalSolverSpecificParameters = null,
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
            { (IlpSolverType.Scip, NumberOfThreadsKey), "parallel/maxnthreads" },
            { (IlpSolverType.HiGhs, RelativeGapKey), "mip_rel_gap" },
            { (IlpSolverType.HiGhs, NumberOfThreadsKey), "threads" }
        };

    private static readonly Dictionary<(LpSolverType, string), string> LpParameterKeyMapping =
        new()
        {
            { (LpSolverType.Glop, RelativeGapKey), "solution_feasibility_tolerance" },
            { (LpSolverType.Glop, NumberOfThreadsKey), "num_omp_threads" },
            { (LpSolverType.GurobiMixedIntegerProgramming, RelativeGapKey), "MIPGap" },
            { (LpSolverType.GurobiMixedIntegerProgramming, NumberOfThreadsKey), "Threads" },
            { (LpSolverType.Scip, RelativeGapKey), "limits/gap" },
            { (LpSolverType.Scip, NumberOfThreadsKey), "parallel/maxnthreads" }
        };

    private static readonly Dictionary<LpSolverType, string> LpKeyValueSeparators =
        new()
        {
            { LpSolverType.Glop, ":" },
            { LpSolverType.GurobiMixedIntegerProgramming, "=" },
            { LpSolverType.Scip, "=" }
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

    internal List<(string Key, string Value)> ToSolverSpecificParametersList(IlpSolverType solverType)
    {
        var parameters = new List<(string Key, string Value)>();
        if (NumberOfThreads is not null)
            parameters.Add((IlpParameterKeyMapping[(solverType, NumberOfThreadsKey)],
                NumberOfThreads.Value.ToString(CultureInfo.InvariantCulture)));
        if (RelativeGap is not null)
            parameters.Add((IlpParameterKeyMapping[(solverType, RelativeGapKey)],
                RelativeGap.Value.ToString(CultureInfo.InvariantCulture)));
        if (AdditionalSolverSpecificParameters is not null) parameters.AddRange(AdditionalSolverSpecificParameters);

        return parameters;
    }

    internal string ToSolverSpecificParameters(IlpSolverType solverType) => string.Join(',',
        ToSolverSpecificParametersList(solverType).Select(parameter => $"{parameter.Key}={parameter.Value}"));

    internal string ToSolverSpecificParameters(LpSolverType solverType)
    {
        var parameters = new List<string>();
        if (NumberOfThreads is not null)
            parameters.Add(
                $"{LpParameterKeyMapping[(solverType, NumberOfThreadsKey)]}{LpKeyValueSeparators[solverType]}{NumberOfThreads.Value}");

        if (AdditionalSolverSpecificParameters is not null)
            parameters.AddRange(AdditionalSolverSpecificParameters.Select(parameter =>
                $"{parameter.Key}{LpKeyValueSeparators[solverType]}{parameter.Value}"));

        return string.Join(',', parameters);
    }

    /// <inderitdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() =>
        $"{nameof(EnableSolverOutput)}: {EnableSolverOutput}, {nameof(RelativeGap)}: {RelativeGap}, {nameof(TimeLimitInMilliseconds)}: {TimeLimitInMilliseconds}, {nameof(NumberOfThreads)}: {NumberOfThreads}, {nameof(ExportModelFilePath)}: {ExportModelFilePath}";
}